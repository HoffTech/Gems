// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
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
                    catch (Exception exception)
                    {
                        this.logger.LogError(exception, "Error in consumer loop");
                    }
                },
                stoppingToken);
        }

        protected virtual THandlerCommand GetConsumerCommand(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
        {
            var command = new THandlerCommand();

            if (!(command is IConsumerRequest<TValue> extCommand))
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
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = this.consumer.Consume(cancellationToken);

                    if (consumeResult == null)
                    {
                        continue;
                    }

                    var command = this.GetConsumerCommand(consumeResult, cancellationToken);

                    // обработать результат чтения топика:
                    await this.mediator.Send(command, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException exception)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    this.logger.LogError(exception, $"ConsumeException {exception.Error.Reason}");

                    if (exception.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception exception)
                {
                    this.logger.LogError(exception, "Unexpected error");
                }
            }

            this.livenessProbe.ServiceIsAlive = false;
        }
    }
}
