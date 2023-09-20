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
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .SelectMany(i => i.GetGenericArguments())
                    .FirstOrDefault(a => a.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)));

                RegisterListener(services, listenerProperty.KeyType, listenerProperty.ValueType, handlerCommandType, listenerProperty.TopicName);
            }
        }

        public static void RegisterListener(IServiceCollection services, Type keyType, Type valueType, Type handlerCommandType, string topicName)
        {
            services.AddSingleton<IHostedService>(
                l =>
                {
                    var mediator = l.GetService<IMediator>();
                    var livenessProbe = l.GetService<ILivenessProbe>();
                    var kafkaConfiguration = l.GetService<IOptions<KafkaConfiguration>>();
                    var loggerFactory = l.GetService<ILoggerFactory>();
                    var listenerType = valueType.Namespace == nameof(System) ? typeof(BaseConsumerListener<,,>) : typeof(CustomConsumerListener<,,>);
                    var typeArgs = new[] { keyType, valueType, handlerCommandType };
                    var constructed = listenerType.MakeGenericType(typeArgs);
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
