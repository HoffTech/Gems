// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using Gems.Jobs.Quartz.Jobs;

using MediatR;

namespace Gems.Jobs.Quartz
{
    public static class JobRegister
    {
        public static ConcurrentDictionary<Type, string> JobInfos { get; set; } = new ConcurrentDictionary<Type, string>();

        public static void RegisterJob<TRequestHandler>(string name, bool isConcurrent)
        {
            RegisterJob(typeof(TRequestHandler), name, isConcurrent);
        }

        public static void RegisterJob(Type type, string name, bool isConcurrent)
        {
            var requestType = typeof(IRequestHandler<>);
            var targetRequestType = type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requestType);
            if (targetRequestType == null)
            {
                throw new NotImplementedException($"You should implement a generic job for name: {name}");
            }

            var jobType = isConcurrent
                ? typeof(ConcurrentQuartzJob<>).MakeGenericType(targetRequestType.GetGenericArguments())
                : typeof(DisallowConcurrentQuartzJob<>).MakeGenericType(targetRequestType.GetGenericArguments());

            if (!JobInfos.TryAdd(jobType, name))
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
    }
}
