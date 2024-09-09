// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

using Gems.Utils.Attributes;

namespace Gems.Utils
{
    /// <summary>
    /// Методы для работы с датами.
    /// </summary>
    public class DateTimeUtils
    {
        public static void SetUnspecifiedToUtcDateTimeEnumerable<TModel>(IEnumerable<TModel> data)
            where TModel : class
        {
            var dateTimeProperties = typeof(TModel).GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(UnspecifiedToUtcDateTimeAttribute)))
                .ToArray();

            foreach (var model in data)
            {
                foreach (var propertyInfo in dateTimeProperties)
                {
                    if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        var dateTimeValue = propertyInfo.GetValue(model, null);
                        if (dateTimeValue != null)
                        {
                            propertyInfo.SetValue(model, ConvertToUtc((DateTime)dateTimeValue), null);
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        var nullable = (DateTime?)propertyInfo.GetValue(model, null);
                        if (nullable.HasValue)
                        {
                            propertyInfo.SetValue(model, ConvertToUtc(nullable.Value), null);
                        }
                    }
                }
            }
        }

        public static void SetUnspecifiedToUtcDateTime<TModel>(TModel model)
            where TModel : class
        {
            var props = model
                .GetType()
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(UnspecifiedToUtcDateTimeAttribute)));

            foreach (var p in props)
            {
                if (p.PropertyType == typeof(DateTime))
                {
                    var date = p.GetValue(model, null);
                    p.SetValue(model, ConvertToUtc((DateTime)date!), null);
                }
                else if (p.PropertyType == typeof(DateTime?))
                {
                    var date = (DateTime?)p.GetValue(model, null);
                    if (date.HasValue)
                    {
                        p.SetValue(model, ConvertToUtc(date.Value), null);
                    }
                }
            }
        }

        public static DateTime ConvertToUtc(DateTime dateTimeValue)
        {
            if (dateTimeValue.Kind != DateTimeKind.Unspecified)
            {
                return dateTimeValue.ToUniversalTime();
            }

            dateTimeValue = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Utc);

            return dateTimeValue;
        }
    }
}
