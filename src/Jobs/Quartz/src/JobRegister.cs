// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using Gems.Jobs.Quartz.Jobs;
using Gems.Jobs.Quartz.Jobs.JobWithData;

using MediatR;

namespace Gems.Jobs.Quartz
{
    public static class JobRegister
    {
        public static ConcurrentDictionary<Type, string> JobNameByRequestTypeMap { get; set; } = new ConcurrentDictionary<Type, string>();

        public static ConcurrentDictionary<Type, string> JobNameByJobTypeMap { get; set; } = new ConcurrentDictionary<Type, string>();

        public static void RegisterJob<TRequestHandler>(string name, bool isConcurrent)
        {
            RegisterJob(typeof(TRequestHandler), name, isConcurrent);
        }

        public static void RegisterJob(Type type, string name, bool isConcurrent)
        {
            var requestHandlerType = typeof(IRequestHandler<>);
            var targetRequestHandlerType = type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requestHandlerType);
            if (targetRequestHandlerType == null)
            {
                throw new NotImplementedException($"You should implement a generic job for name: {name}");
            }

            var targetRequestHandlerTypeArguments = targetRequestHandlerType.GetGenericArguments();
            var requestType = targetRequestHandlerTypeArguments.FirstOrDefault();

            var jobType = GetJobType(requestType, targetRequestHandlerTypeArguments, isConcurrent);

            if (!JobNameByJobTypeMap.TryAdd(jobType, name) || !JobNameByRequestTypeMap.TryAdd(requestType, name))
            {
                throw new InvalidOperationException($"Failed register job: {name}");
            }
        }

        public static void RegisterJobs(params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(x => x.GetCustomAttributes<JobHandlerAttribute>().Any())
                .ToList();

            foreach (var type in types)
            {
                var jobAttribute = type.GetCustomAttributes<JobHandlerAttribute>().First();
                RegisterJob(type, jobAttribute.Name, jobAttribute.IsConcurrent);
            }
        }

        private static Type GetJobType(Type requestType, Type[] targetRequestHandlerTypeArguments, bool isConcurrent)
        {
            var commandHasProperties = requestType?.GetProperties().Any() ?? false;

            if (commandHasProperties)
            {
                return isConcurrent
                    ? typeof(ConcurrentQuartzJobWithData<>).MakeGenericType(targetRequestHandlerTypeArguments)
                    : typeof(DisallowConcurrentQuartzJobWithData<>).MakeGenericType(targetRequestHandlerTypeArguments);
            }

            return isConcurrent
                ? typeof(ConcurrentQuartzJob<>).MakeGenericType(targetRequestHandlerTypeArguments)
                : typeof(DisallowConcurrentQuartzJob<>).MakeGenericType(targetRequestHandlerTypeArguments);
        }
    }
}
