// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using MediatR;

namespace Gems.Mvc.GenericControllers
{
    public static class ControllerRegister
    {
        public static ConcurrentDictionary<Type, EndpointAttribute> ControllerInfos { get; set; } = new ConcurrentDictionary<Type, EndpointAttribute>();

        public static void RegisterController<TRequestHandler>(string route, string method)
        {
            RegisterController<TRequestHandler>(route, method, null);
        }

        public static void RegisterController<TRequestHandler>(string route, string method, string endpointStartForHiding)
        {
            RegisterController(typeof(TRequestHandler), new EndpointAttribute(route, method), endpointStartForHiding);
        }

        public static void RegisterController(Type type, EndpointAttribute endpoint)
        {
            RegisterController(type, endpoint, null);
        }

        public static void RegisterController(Type type, EndpointAttribute endpoint, string endpointStartForHiding)
        {
            if (!string.IsNullOrEmpty(endpointStartForHiding) && endpoint.Route.StartsWith(endpointStartForHiding))
            {
                return;
            }

            var requestType = typeof(IRequestHandler<>);
            var targetRequestType = type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requestType);
            if (targetRequestType != null)
            {
                var controllerType = GetGenericControllerType(endpoint).MakeGenericType(targetRequestType.GetGenericArguments());
                ControllerInfos.AddOrUpdate(controllerType, t => endpoint, (t, v) => endpoint);
                return;
            }

            var requestTypeWithResponse = typeof(IRequestHandler<,>);
            targetRequestType = type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == requestTypeWithResponse);
            if (targetRequestType != null)
            {
                var controllerType = GetGenericWithResponseControllerType(endpoint).MakeGenericType(targetRequestType.GetGenericArguments());
                ControllerInfos.AddOrUpdate(controllerType, t => endpoint, (t, v) => endpoint);
                return;
            }

            throw new NotImplementedException($"You should implement a generic controller for method: {endpoint.Method}");
        }

        public static void RegisterControllers(params Assembly[] assemblies)
        {
            RegisterControllers(null, assemblies);
        }

        public static void RegisterControllers(string endpointStartForHiding, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetExecutingAssembly() };
            }

            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetCustomAttributes<EndpointAttribute>().Any());
            foreach (var type in types)
            {
                var endpoint = type.GetCustomAttributes<EndpointAttribute>().First();
                RegisterController(type, endpoint, endpointStartForHiding);
            }
        }

        private static Type GetGenericWithResponseControllerType(EndpointAttribute endpoint)
        {
            return endpoint.Method.ToUpper() switch
            {
                "GET" => typeof(QueryController<,>),
                "POST" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PostFormSourceCommandWithResponseController<,>) :
                    endpoint.SourceType == SourceType.FromQuery ? typeof(PostQuerySourceCommandWithResponseController<,>)
                    : typeof(PostBodySourceCommandWithResponseController<,>),
                "PUT" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PutFormSourceCommandWithResponseController<,>)
                    : endpoint.SourceType == SourceType.FromQuery ? typeof(PutQuerySourceCommandWithResponseController<,>)
                    : typeof(PutBodySourceCommandWithResponseController<,>),
                "PATCH" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PatchFormSourceCommandWithResponseController<,>)
                    : endpoint.SourceType == SourceType.FromQuery ? typeof(PatchQuerySourceCommandWithResponseController<,>)
                    : typeof(PatchBodySourceCommandWithResponseController<,>),
                _ => throw new NotImplementedException()
            };
        }

        private static Type GetGenericControllerType(EndpointAttribute endpoint)
        {
            return endpoint.Method.ToUpper() switch
            {
                "POST" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PostFormSourceCommandController<>)
                    : endpoint.SourceType == SourceType.FromQuery ? typeof(PostQuerySourceCommandController<>)
                    : typeof(PostBodySourceCommandController<>),
                "PUT" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PutFormSourceCommandController<>)
                    : endpoint.SourceType == SourceType.FromQuery ? typeof(PutQuerySourceCommandController<>)
                    : typeof(PutBodySourceCommandController<>),
                "PATCH" => endpoint.IsForm || endpoint.SourceType == SourceType.FromForm ? typeof(PatchFormSourceCommandController<>)
                    : endpoint.SourceType == SourceType.FromQuery ? typeof(PatchQuerySourceCommandController<>)
                    : typeof(PatchBodySourceCommandController<>),
                "DELETE" => typeof(DeleteQuerySourceCommandController<>),
                _ => throw new NotImplementedException()
            };
        }
    }
}
