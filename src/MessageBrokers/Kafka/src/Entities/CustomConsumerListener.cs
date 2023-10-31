// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Threading;

using Confluent.Kafka;

using Gems.HealthChecks;
using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;
using Gems.MessageBrokers.Kafka.Entities.KafkaBuilder;
using Gems.Text.Json;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.MessageBrokers.Kafka.Entities
{
    public class CustomConsumerListener<TKey, TValue, THandlerCommand> : BaseConsumerListener<TKey, TValue, THandlerCommand>
        where TValue : class
        where THandlerCommand : class, IBaseRequest, new()
    {
        public CustomConsumerListener(
            IMediator mediator,
            LoggerFactory loggerFactory,
            ILivenessProbe livenessProbe,
            IOptions<KafkaConfiguration> kafkaConfiguration,
            string topic)
            : base(mediator, loggerFactory, livenessProbe, kafkaConfiguration, topic)
        {
        }

        protected override THandlerCommand GetConsumerCommand(ConsumeResult<TKey, TValue> consumeResult, CancellationToken cancellationToken)
        {
            if (typeof(THandlerCommand) == typeof(TValue))
            {
                return consumeResult.Message.Value as THandlerCommand;
            }

            return consumeResult.Message.Value is string valueAsString
                ? DeserializeCommand(valueAsString)
                : throw new InvalidOperationException();
        }

        protected override BaseConsumerClientBuilder<TKey, TValue> GetConsumerClientBuilder(KafkaConfiguration configuration, string topic)
        {
            return new CustomConsumerClientBuilder<TKey, TValue>(configuration, topic);
        }

        private static THandlerCommand DeserializeCommand(string value)
        {
            var bracePosition = value.IndexOf("{", StringComparison.Ordinal);
            return bracePosition switch
            {
                -1 => throw new InvalidOperationException($"Не удалось распарсить команду: {typeof(THandlerCommand).Name}"),
                0 => value.Deserialize<THandlerCommand>(),
                _ => value[bracePosition..].Deserialize<THandlerCommand>()
            };
        }
    }
}
