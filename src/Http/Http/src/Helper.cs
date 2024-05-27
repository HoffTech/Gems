// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Http
{
    /// <summary>
    /// Вспомогательный класс.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Проверяет валидность урла.
        /// </summary>
        /// <param name="uri">урл для проверки.</param>
        /// <returns>результат проверки.</returns>
        public static bool CheckUrlIsValid(string uri)
        {
            var result = Uri.TryCreate(uri, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        /// <summary>
        /// Удалить после отладки.
        /// </summary>
        public static void Kmp(string uri)
        {
            var result = Uri.TryCreate(uri, UriKind.Relative, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
