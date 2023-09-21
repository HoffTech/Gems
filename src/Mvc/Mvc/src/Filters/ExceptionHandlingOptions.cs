// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Mvc.Filters
{
    public class ExceptionHandlingOptions
    {
        /// <summary>
        /// Name in appsettings.json.
        /// </summary>
        public const string SectionName = "ExceptionHandlingOptions";

        /// <summary>
        /// Опция включения стандартной фильтрации ошибок для Vertical Slice.
        /// </summary>
        /// <remarks>
        /// По умолчанию включено.
        /// </remarks>
        public bool UseHandleErrorFilterOnNonGenericControllers { get; set; } = true;
    }
}
