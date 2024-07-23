// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Gems.Text.Json.Converters
{
    public class UnspecifiedDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DefaultDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public UnspecifiedDateTimeConverter()
        {
            this.TargetTimeKind = DateTimeKind.Unspecified;
        }

        public virtual DateTimeKind TargetTimeKind { get; set; }

        public virtual string SerializerFormat { get; set; }

        public virtual string DeserializerFormat { get; set; }

        public virtual string SerializerTimeZone { get; set; }

        public virtual string DeserializerTimeZone { get; set; }

        public virtual bool DisableTreatmentMilliseconds { get; set; }

        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var dateTimeAsString = reader.GetString();
            try
            {
                var dateTimeFormat = this.DeserializerFormat;

                this.CheckAndSetMilliseconds(ref dateTimeFormat, ref dateTimeAsString);
                if (this.DeserializerTimeZone != null)
                {
                    dateTimeFormat ??= DefaultDateTimeFormat;
                    dateTimeFormat = dateTimeFormat.Replace("z", string.Empty);
                    dateTimeFormat += "zzz";
                    dateTimeAsString += GetOffsetByTimeZone(this.DeserializerTimeZone);
                }

                var dateTimeValue = string.IsNullOrEmpty(dateTimeFormat)
                    ? DateTime.Parse(dateTimeAsString!)
                    : DateTime.ParseExact(dateTimeAsString!, dateTimeFormat, CultureInfo.InvariantCulture);
                dateTimeValue = this.SpecifyDateTimeKind(dateTimeValue);

                // Возвращаем приложению дату всегда в UTC
                dateTimeValue = dateTimeValue.ToUniversalTime();
                return dateTimeValue;
            }
            catch (Exception e)
            {
                throw new Exception($"Не удалось разобрать дату: {dateTimeAsString}", e);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options)
        {
            var dateTimeFormat = this.SerializerFormat ?? DefaultDateTimeFormat;
            dateTimeValue = this.SpecifyDateTimeKind(dateTimeValue);
            if (!string.IsNullOrEmpty(this.SerializerTimeZone))
            {
                dateTimeFormat = dateTimeFormat.Replace("z", string.Empty);
                dateTimeValue = dateTimeValue.Kind == DateTimeKind.Utc
                    ? dateTimeValue.ToLocalTime()
                    : dateTimeValue;
                dateTimeValue = TimeZoneInfo.ConvertTime(dateTimeValue, TimeZoneInfoHelper.FindSystemTimeZoneById(this.SerializerTimeZone));
                var dateTimeAsString = dateTimeValue.ToString(dateTimeFormat);
                dateTimeAsString += GetOffsetByTimeZone(this.SerializerTimeZone);
                writer.WriteStringValue(dateTimeAsString);
            }
            else
            {
                writer.WriteStringValue(dateTimeValue.ToString(dateTimeFormat));
            }
        }

        private static string GetOffsetByTimeZone(string timeZoneId)
        {
            var offset = TimeZoneInfoHelper.FindSystemTimeZoneById(timeZoneId)?.BaseUtcOffset.ToString()[0..^3];
            if (offset.Length == 5)
            {
                offset = $"+{offset}";
            }

            return offset;
        }

        private void CheckAndSetMilliseconds(ref string dateTimeFormat, ref string dateTimeAsString)
        {
            if (this.DisableTreatmentMilliseconds)
            {
                return;
            }

            var millisecondsFormatRegex = new Regex(@":ss\.f+");
            var millisecondsRegex = new Regex(@":\d+\.\d+");

            if (string.IsNullOrEmpty(dateTimeFormat) || string.IsNullOrEmpty(dateTimeAsString))
            {
                return;
            }

            if (!millisecondsFormatRegex.IsMatch(dateTimeFormat) || !millisecondsRegex.IsMatch(dateTimeAsString))
            {
                return;
            }

            var millisecondsFormat = millisecondsFormatRegex.Match(dateTimeFormat).Value;
            var milliseconds = millisecondsRegex.Match(dateTimeAsString).Value;

            const int maxMillisecondsLimitInDtWithSecondsAndDot = 11;
            if (millisecondsFormat.Length > maxMillisecondsLimitInDtWithSecondsAndDot)
            {
                millisecondsFormat = millisecondsFormat[..maxMillisecondsLimitInDtWithSecondsAndDot];
                dateTimeFormat = millisecondsFormatRegex.Replace(dateTimeFormat, millisecondsFormat);
            }

            if (millisecondsFormat.Length == milliseconds.Length)
            {
                return;
            }

            if (millisecondsFormat.Length <= milliseconds.Length)
            {
                return;
            }

            var correctMilliseconds = milliseconds.PadRight(millisecondsFormat.Length, '0');
            dateTimeAsString = millisecondsRegex.Replace(dateTimeAsString, correctMilliseconds);
        }

        private DateTime SpecifyDateTimeKind(DateTime dateTimeValue)
        {
            if (dateTimeValue.Kind != DateTimeKind.Unspecified)
            {
                return dateTimeValue;
            }

            dateTimeValue = DateTime.SpecifyKind(dateTimeValue, this.TargetTimeKind);

            return dateTimeValue;
        }
    }
}
