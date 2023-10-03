// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using Dapper;

namespace Gems.Data.MySql
{
    public static class MySqlMapper
    {
        public static void RegisterMappers(Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetCustomAttributes<MySqlTypeAttribute>().Any());
            foreach (var type in types)
            {
                RegisterMapper(type);
            }
        }

        public static void RegisterMappers(Assembly assembly)
        {
            RegisterMappers(new[] { assembly });
        }

        public static void RegisterMapper(Type type)
        {
            var map = new CustomPropertyTypeMap(type, (type, columnName)
                => type.GetProperties().FirstOrDefault(prop => GetColumnNameFromAttribute(prop) == columnName.ToLower()));
            SqlMapper.SetTypeMap(type, map);
        }

        private static string GetColumnNameFromAttribute(MemberInfo member)
        {
            if (member == null)
            {
                return null;
            }

            var attr = member.GetCustomAttributes(typeof(ColumnAttribute)).Cast<ColumnAttribute>().FirstOrDefault();
            return (attr?.Name ?? member.Name).ToLower();
        }
    }
}
