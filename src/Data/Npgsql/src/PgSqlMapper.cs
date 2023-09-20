// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;

using Dapper;

using Npgsql;

using NpgsqlTypes;

namespace Gems.Data.Npgsql
{
    public static class PgSqlMapper
    {
        public static void RegisterMappers(Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetCustomAttributes<PgTypeAttribute>().Any());
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
            var pgTypeAttribute = type.GetCustomAttributes<PgTypeAttribute>().First();
            RegisterMapper(type, pgTypeAttribute.TypeName);
        }

        private static void RegisterMapper(Type type, string pgType)
        {
            if (!string.IsNullOrEmpty(pgType))
            {
                RegisterMapperForInput(type, pgType);
            }

            RegisterMapperForOutput(type);
        }

        private static void RegisterMapperForInput(Type type, string pgType)
        {
            NpgsqlConnection.GlobalTypeMapper.MapComposite(type, pgType);
        }

        private static void RegisterMapperForOutput(Type type)
        {
            var map = new CustomPropertyTypeMap(type, (type, columnName)
                => type.GetProperties().FirstOrDefault(prop => GetPgNameFromAttribute(prop) == columnName.ToLower()));
            SqlMapper.SetTypeMap(type, map);
        }

        private static string GetPgNameFromAttribute(MemberInfo member)
        {
            if (member == null)
            {
                return null;
            }

            var attr = member.GetCustomAttributes(typeof(PgNameAttribute)).Cast<PgNameAttribute>().FirstOrDefault();
            return (attr?.PgName ?? member.Name).ToLower();
        }
    }
}
