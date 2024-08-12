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
            var millisecondsValueRegex = new Regex(@":\d\d\.\d+");

            if (string.IsNullOrEmpty(dateTimeFormat) || string.IsNullOrEmpty(dateTimeAsString))
            {
                return;
            }

            if (!millisecondsFormatRegex.IsMatch(dateTimeFormat) && !millisecondsValueRegex.IsMatch(dateTimeAsString))
            {
                return;
            }

            if (!millisecondsFormatRegex.IsMatch(dateTimeFormat))
            {
                var formatWithoutMillisecondsRegex = new Regex(":ss");
                if (!formatWithoutMillisecondsRegex.IsMatch(dateTimeFormat))
                {
                    return;
                }

                dateTimeFormat = formatWithoutMillisecondsRegex.Replace(dateTimeFormat, ":ss.f");
            }

            if (!millisecondsValueRegex.IsMatch(dateTimeAsString))
            {
                var valueWithoutMillisecondsRegex = new Regex(@"\d\d:\d\d:\d\d");
                if (!valueWithoutMillisecondsRegex.IsMatch(dateTimeAsString))
                {
                    return;
                }

                var valueWithoutMilliseconds = valueWithoutMillisecondsRegex.Match(dateTimeAsString).Value + ".0";
                dateTimeAsString = valueWithoutMillisecondsRegex.Replace(dateTimeAsString, valueWithoutMilliseconds);
            }

            if (!millisecondsFormatRegex.IsMatch(dateTimeFormat) || !millisecondsValueRegex.IsMatch(dateTimeAsString))
            {
                return;
            }

            var millisecondsFormat = millisecondsFormatRegex.Match(dateTimeFormat).Value;
            var millisecondsValue = millisecondsValueRegex.Match(dateTimeAsString).Value;

            const int maxMillisecondsLimitInDtWithSecondsAndDot = 11;
            if (millisecondsFormat.Length > maxMillisecondsLimitInDtWithSecondsAndDot)
            {
                millisecondsFormat = millisecondsFormat[..maxMillisecondsLimitInDtWithSecondsAndDot];
                dateTimeFormat = millisecondsFormatRegex.Replace(dateTimeFormat, millisecondsFormat);
            }

            if (millisecondsValue.Length > maxMillisecondsLimitInDtWithSecondsAndDot)
            {
                millisecondsValue = millisecondsValue[..maxMillisecondsLimitInDtWithSecondsAndDot];
                dateTimeAsString = millisecondsValueRegex.Replace(dateTimeAsString, millisecondsValue);
            }

            if (millisecondsFormat.Length < millisecondsValue.Length)
            {
                var correctMillisecondsFormat = millisecondsFormat.PadRight(millisecondsValue.Length, 'f');
                dateTimeFormat = millisecondsFormatRegex.Replace(dateTimeFormat, correctMillisecondsFormat);
                return;
            }

            if (millisecondsFormat.Length > millisecondsValue.Length)
            {
                var correctMillisecondsValue = millisecondsValue.PadRight(millisecondsFormat.Length, '0');
                dateTimeAsString = millisecondsValueRegex.Replace(dateTimeAsString, correctMillisecondsValue);
                return;
            }

            if (millisecondsFormat.Length == millisecondsValue.Length)
            {
                return;
            }

            throw new Exception("Что-то пошло не так!");
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
