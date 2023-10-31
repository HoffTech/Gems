// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;

using Gems.HealthChecks;
using Gems.MessageBrokers.Kafka.AppData.KafkaOptions;
using Gems.MessageBrokers.Kafka.Entities;
using Gems.MessageBrokers.Kafka.Interfaces;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gems.MessageBrokers.Kafka.Configuration
{
    /// <summary>
    /// Class provides methods to registrate consumers and producers.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Создание, конфигурация и регистрация консьюмера в коллекции сервисов IServiceCollection.
        /// </summary>
        /// <param name="services">коллекция сервисов.</param>
        /// <param name="configuration">configuration.</param>
        public static void AddConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConfiguration>(configuration.GetSection(nameof(KafkaConfiguration)));
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetCustomAttributes<ConsumerListenerPropertyAttribute>().Any());

            foreach (var type in types)
            {
                var listenerProperty = type.GetCustomAttributes<ConsumerListenerPropertyAttribute>().First();

                var handlerCommandType = type
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                    .SelectMany(i => i.GetGenericArguments())
                    .FirstOrDefault(a => a.GetInterfaces().Any(x => x == typeof(IBaseRequest)));

                var consumerRequestType = handlerCommandType!
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumerRequest<>));

                Type consumerListenerType;
                if (consumerRequestType != null)
                {
                    var consumerRequestValueType = consumerRequestType.GetGenericArguments().First();
                    if (listenerProperty.ValueType != null && listenerProperty.ValueType != consumerRequestValueType)
                    {
                        throw new ArgumentException($"Тип ConsumerListenerPropertyAttribute.ValueType должен быть равен типу аргумента интерфейса IConsumerRequest<>.");
                    }

                    if (listenerProperty.NeedParseJsonFromString)
                    {
                        throw new ArgumentException($"Аттрибут NeedParseJsonFromString не может быть установлен, если у команды или запроса определен интерфейс IConsumerRequest;{handlerCommandType.Name}");
                    }

                    listenerProperty.ValueType ??= consumerRequestValueType;
                    consumerListenerType = typeof(BaseConsumerListener<,,>);
                }
                else
                {
                    if (listenerProperty.NeedParseJsonFromString && !(listenerProperty.ValueType == null || listenerProperty.ValueType == typeof(string)))
                    {
                        throw new ArgumentException($"Аттрибут NeedParseJsonFromString не может быть установлен, если ConsumerListenerPropertyAttribute.ValueType не равен System.String");
                    }

                    if (!listenerProperty.NeedParseJsonFromString && listenerProperty.ValueType != null && listenerProperty.ValueType != handlerCommandType)
                    {
                        throw new ArgumentException($"Тип ConsumerListenerPropertyAttribute.ValueType должен быть равен типу команды/запроса {handlerCommandType.Name}.");
                    }

                    listenerProperty.ValueType = listenerProperty.NeedParseJsonFromString
                        ? typeof(string)
                        : handlerCommandType;
                    consumerListenerType = typeof(CustomConsumerListener<,,>);
                }

                RegisterListener(
                    services,
                    listenerProperty.KeyType,
                    listenerProperty.ValueType,
                    handlerCommandType,
                    listenerProperty.TopicName,
                    consumerListenerType);
            }
        }

        public static void RegisterListener(IServiceCollection services, Type keyType, Type valueType, Type handlerCommandType, string topicName, Type consumerListenerType)
        {
            services.AddSingleton<IHostedService>(
                l =>
                {
                    var mediator = l.GetService<IMediator>();
                    var livenessProbe = l.GetService<ILivenessProbe>();
                    var kafkaConfiguration = l.GetService<IOptions<KafkaConfiguration>>();
                    var loggerFactory = l.GetService<ILoggerFactory>();
                    var typeArgs = new[] { keyType, valueType, handlerCommandType };
                    var constructed = consumerListenerType.MakeGenericType(typeArgs);
                    var instance = Activator.CreateInstance(constructed, mediator, loggerFactory, livenessProbe, kafkaConfiguration, topicName) as IHostedService;
                    return instance;
                });
        }

        /// <summary>
        /// Создание, конфигурация и регистрация продюсеров в коллекции сервисов IServiceCollection.
        /// </summary>
        /// <param name="services">коллекция сервисов.</param>
        /// <param name="configuration">configuration.</param>
        public static void AddProducers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConfiguration>(configuration.GetSection(nameof(KafkaConfiguration)));
            services.AddSingleton<IMessageProducer, MessageProducer>();
        }
    }
}
