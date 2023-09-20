// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Mvc.GenericControllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndpointAttribute : Attribute
    {
        public EndpointAttribute(string route, string method)
        {
            this.Route = route;
            this.Method = method.ToUpper();
        }

        /// <summary>
        /// Полный URL.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Http метод.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Указывает к какой группе относится данный эндпоинт. Необходимо для отображения в OpenApi (swagger).
        /// </summary>
        public string OperationGroup { get; set; }

        /// <summary>
        /// Является ли тело запроса формой. По умолчанию false.
        /// </summary>
        public bool IsForm { get; set; }

        /// <summary>
        /// Тип источника данных. По умолчанию FromBody.
        /// </summary>
        public SourceType SourceType { get; set; } = SourceType.FromBody;

        /// <summary>
        /// Краткое описание метода.
        /// </summary>
        public string Summary { get; set; }
    }
}
