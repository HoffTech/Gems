// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.Json.Serialization;

namespace Gems.Text.Json.Converters
{
    public class UnspecifiedDateTimeConverterAttribute : JsonConverterAttribute
    {
        public DateTimeKind TargetTimeKind { get; set; }

        public string SerializerFormat { get; set; }

        public string DeserializerFormat { get; set; }

        public string SerializerTimeZone { get; set; }

        public string DeserializerTimeZone { get; set; }

        public bool DisableTreatmentMilliseconds { get; set; }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(DateTime))
            {
                throw new ArgumentException("DateTime type must be specified.");
            }

            var converter = new UnspecifiedDateTimeConverter();
            if (this.TargetTimeKind != DateTimeKind.Unspecified)
            {
                converter.TargetTimeKind = this.TargetTimeKind;
            }

            if (this.SerializerFormat != null)
            {
                converter.SerializerFormat = this.SerializerFormat;
            }

            if (this.DeserializerFormat != null)
            {
                converter.DeserializerFormat = this.DeserializerFormat;
            }

            if (this.SerializerTimeZone != null)
            {
                converter.SerializerTimeZone = this.SerializerTimeZone;
            }

            if (this.DeserializerTimeZone != null)
            {
                converter.DeserializerTimeZone = this.DeserializerTimeZone;
            }

            converter.DisableTreatmentMilliseconds = this.DisableTreatmentMilliseconds;

            return converter;
        }
    }
}
