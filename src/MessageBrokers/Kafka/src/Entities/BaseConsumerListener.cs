// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;

using Gems.HealthChecks;
using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;
using Gems.MessageBrokers.Kafka.Entities.KafkaBuilder;
using Gems.MessageBrokers.Kafka.Interfaces;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.MessageBrokers.Kafka.Entities
{
    public class BaseConsumerListener<TKey, TValue, THandlerCommand> : BackgroundService
        where THandlerCommand : class, IBaseRequest, new()
    {
        private readonly IMediator mediator;
        private readonly ILogger logger;
        private readonly KafkaConfiguration kafkaConfiguration;
        private readonly string topic;
        private readonly ILivenessProbe livenessProbe;
        private IConsumer<TKey, TValue> consumer;
        private Queue<int> retryAttemptDelaysQueue;

        /// <summary>
        /// Инициализирует новый экземпляр класса .
        /// </summary>
        /// <param name="mediator">медиатор.</param>
        /// <param name="loggerFactory">loggerFactory.</param>
        /// <param name="livenessProbe">интерфейс сервиса для проверки состояния сервиса.</param>
        /// <param name="kafkaConfiguration">конфиг кафки.</param>
        /// <param name="topic">наименование топика.</param>
        public BaseConsumerListener(
            IMediator mediator,
            LoggerFactory loggerFactory,
            ILivenessProbe livenessProbe,
            IOptions<KafkaConfiguration> kafkaConfiguration,
            string topic)
        {
            this.mediator = mediator;
            this.logger = loggerFactory.CreateLogger<BaseConsumerListener<TKey, TValue, THandlerCommand>>();
            this.livenessProbe = livenessProbe;
            this.kafkaConfiguration = kafkaConfiguration.Value;
            this.topic = topic;

            this.InitConsumerInstance();
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            this.consumer.Close();

            this.consumer.Dispose();

            this.logger.LogInformation("ConsumerListener is disposed");

            base.Dispose();
        }

        /// <inheritdoc/>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("ConsumerListener stopped");

            return base.StopAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("ConsumerListener started");

            return base.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(
                async () =>
                {
                    try
                    {
                        await this.StartConsumerLoop(stoppingToken)
                            .ConfigureAwait(false);
                    }
                    finally
                    {
                        this.livenessProbe.ServiceIsAlive = false;
                        this.consumer?.Close();
                    }
                },
                stoppingToken);
        }

        protected virtual THandlerCommand GetConsumerCommand(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
        {
            var command = new THandlerCommand();
            if (command is not IConsumerRequest<TValue> extCommand)
            {
                return command;
            }

            extCommand.Value = consumeResult.Message.Value;

            return (THandlerCommand)extCommand;
        }

        protected virtual BaseConsumerClientBuilder<TKey, TValue> GetConsumerClientBuilder(KafkaConfiguration configuration, string topic)
        {
            return new BaseConsumerClientBuilder<TKey, TValue>(configuration, topic);
        }

        private void InitConsumerInstance()
        {
            var builder = this.GetConsumerClientBuilder(this.kafkaConfiguration, this.topic);
            this.consumer = builder.BuildTypeConsumer();
        }

        /// <summary>
        /// Consumer start to listening kafka queue in while loop.
        /// </summary>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>Task result.</returns>
        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            TopicPartitionOffset currentTopicPartitionOffset = null;
            while (true)
            {
                try
                {
                    var consumeResult = this.consumer.Consume(cancellationToken);
                    if (consumeResult == null)
                    {
                        throw new InvalidOperationException("ConsumeResult is null");
                    }

                    currentTopicPartitionOffset = consumeResult.TopicPartitionOffset;
                    this.logger.LogInformation(
                        "ConsumeResult {TopicPartitionOffset} executing at {Time}",
                        currentTopicPartitionOffset,
                        DateTime.UtcNow);

                    var command = this.GetConsumerCommand(consumeResult, cancellationToken);
                    this.SetMetadata(consumeResult, command);

                    // обработать результат чтения топика:
                    await this.mediator.Send(command, cancellationToken).ConfigureAwait(false);

                    this.CommitConsumeResult(consumeResult);
                }
                catch (OperationCanceledException exception)
                {
                    this.logger.LogError(exception, "Consumer canceled.");
                    throw;
                }
                catch (ConsumeException exception) when (exception.Error.IsFatal)
                {
                    this.logger.LogError(exception, $"ConsumeException {exception.Error.Reason}");
                    throw;
                }
                catch (ConsumeException exception)
                {
                    this.logger.LogError(exception, $"ConsumeException {exception.Error.Reason}");
                    await this.RetryConsume(currentTopicPartitionOffset).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Unexpected error");
                    await this.RetryConsume(currentTopicPartitionOffset).ConfigureAwait(false);
                }
                finally
                {
                    this.logger.LogInformation(
                        "ConsumeResult {TopicPartitionOffset} executed at {Time}",
                        currentTopicPartitionOffset,
                        DateTime.UtcNow);
                }
            }
        }

        private void SetMetadata(ConsumeResult<TKey, TValue> consumeResult, THandlerCommand command)
        {
            if (command is not IHasMessageMetadata<TKey> commandWithMetadata)
            {
                return;
            }

            commandWithMetadata.Key = consumeResult.Message.Key;
            commandWithMetadata.Offset = consumeResult.Offset;
            commandWithMetadata.Partition = consumeResult.Partition;
            commandWithMetadata.Headers = consumeResult.Message.Headers
                .Select(x => new
                {
                    name = x.Key,
                    value = Encoding.UTF8.GetString(x.GetValueBytes())
                })
                .ToDictionary(x => x.name, y => y.value);
        }

        private void CommitConsumeResult(ConsumeResult<TKey, TValue> consumeResult)
        {
            if (this.kafkaConfiguration.Consumers[this.topic].EnableAutoCommit == false)
            {
                this.consumer.Commit(consumeResult);
            }

            if (this.kafkaConfiguration.Consumers[this.topic].EnableAutoOffsetStore == false)
            {
                this.consumer.StoreOffset(consumeResult);
            }

            this.retryAttemptDelaysQueue = null;
        }

        private async Task RetryConsume(TopicPartitionOffset currentTopicPartitionOffset)
        {
            if (currentTopicPartitionOffset == null)
            {
                throw new InvalidOperationException("TopicPartitionOffset is null");
            }

            var consumerSettings = this.kafkaConfiguration.Consumers[this.topic];
            if (!(consumerSettings.EnableRetry ?? false))
            {
                return;
            }

            this.retryAttemptDelaysQueue ??= this.CreateRetryAttemptsQueue();
            var delay = this.retryAttemptDelaysQueue.Count > 0
                ? this.retryAttemptDelaysQueue.Dequeue()
                : consumerSettings.RetryAttempts[^1].DelayInMilliseconds;

            this.consumer.Assign(currentTopicPartitionOffset);
            await Task.Delay(delay).ConfigureAwait(false);
        }

        private Queue<int> CreateRetryAttemptsQueue()
        {
            var retryAttemptDelaysQueue = new Queue<int>();
            foreach (var retryAttempts in this.kafkaConfiguration.Consumers[this.topic].RetryAttempts)
            {
                var countAttempts = retryAttempts.CountAttempts <= 0 ? 1 : retryAttempts.CountAttempts;
                for (var i = 0; i < countAttempts; i++)
                {
                    retryAttemptDelaysQueue.Enqueue(retryAttempts.DelayInMilliseconds);
                }
            }

            return retryAttemptDelaysQueue;
        }
    }
}
