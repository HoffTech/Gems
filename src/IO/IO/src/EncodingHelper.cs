// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.IO;
using System.Text;

namespace Gems.IO
{
    public static class EncodingHelper
    {
        /// <summary>
        /// Получает кодировку файла по переданному коду страницы и в случае ошибки возвращает кодировку по умолчанию.
        /// </summary>
        /// <param name="file">Файловый поток.</param>
        /// <param name="encodingCodePage">Код кодировки.</param>
        /// <param name="defaultEncodingCodePage">Код кодировки по умолчанию в случае ошибки.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Страницы кодировок - https://learn.microsoft.com/en-us/windows/win32/intl/code-page-identifiers</item>
        /// <item>Если используются кодировка ANSI, перед вызовом метода зарегистрируйте провайдер Encoding.RegisterProvider(CodePagesEncodingProvider.Instance).</item>
        /// </list>
        /// </remarks>
        /// <returns>Информация о кодировке.</returns>
        public static Encoding GetFileEncoding(Stream file, int encodingCodePage, int defaultEncodingCodePage)
        {
            var encodingVerifier = Encoding.GetEncoding(
                encodingCodePage,
                new EncoderExceptionFallback(),
                new DecoderExceptionFallback());
            using var reader = new StreamReader(
                file,
                encodingVerifier,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true,
                bufferSize: 1024);

            Encoding detectedEncoding;
            try
            {
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                }

                detectedEncoding = reader.CurrentEncoding;
            }
            catch
            {
                detectedEncoding = Encoding.GetEncoding(defaultEncodingCodePage);
            }

            file.Seek(0, SeekOrigin.Begin);
            return detectedEncoding;
        }
    }
}
