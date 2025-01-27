// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gems.Logging.Security
{
    public class ObjectPropertyFilterSource : ISecureKeySource, IObjectPropertyFilterSource
    {
        private readonly List<SecureKey> secureKeys = new List<SecureKey>();
        private readonly HashSet<Type> types = new HashSet<Type>();

        public List<SecureKey> Keys()
        {
            return this.secureKeys;
        }

        public void Reset()
        {
            this.secureKeys.Clear();
        }

        public void Register(Type type)
        {
            if (this.types.Contains(type))
            {
                return;
            }

            this.types.Add(type);
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var filterAttribute = prop.GetCustomAttribute(typeof(FilterAttribute), true) as FilterAttribute;
                if (filterAttribute != null)
                {
                    var matcher = new ObjectPropertyMatcher(type, prop.Name);
                    this.secureKeys.Add(new SecureKey(
                        matcher,
                        string.IsNullOrEmpty(filterAttribute.Search) ? SecureKeyAction.Remove : SecureKeyAction.Update,
                        string.IsNullOrEmpty(filterAttribute.Search) ? null : new Regex(filterAttribute.Search),
                        string.IsNullOrEmpty(filterAttribute.Search) ? null : filterAttribute.Replace));
                }

                var propType = prop.PropertyType;

                // Обработка массивов и коллекций
                if (propType.IsArray || (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    var elementType = propType.IsArray ? propType.GetElementType() : propType.GetGenericArguments()[0];
                    this.Register(elementType); // Рекурсивно регистрировать элементы массива
                }
                else if (propType.GetTypeInfo().IsClass)
                {
                    this.Register(propType);
                }
            }
        }
    }
}
