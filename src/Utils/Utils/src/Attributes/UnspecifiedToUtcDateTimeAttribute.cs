// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Utils.Attributes
{
    /// <summary>
    /// Аттрибут для назначения даты и времени, которую необходимо сконвертировать в UTC при получении из БД.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UnspecifiedToUtcDateTimeAttribute : Attribute
    {
    }
}
