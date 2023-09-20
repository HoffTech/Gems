// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Text.Json.Converters
{
    public class UnspecifiedToLocalDateTimeConverter : UnspecifiedDateTimeConverter
    {
        public override DateTimeKind TargetTimeKind => DateTimeKind.Local;
    }
}
