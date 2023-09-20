// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.IO.LibreOffice.Options
{
    public class LibreOfficeOptions
    {
        /// <summary>
        /// Путь к исполняемому файлу LibreOffice.
        /// </summary>
        public string LibreOfficeExecutablePath { get; set; }

        /// <summary>
        /// Путь к папке для генерации временных пользователей LibreOffice для аргументов команды.
        /// </summary>
        public string TempUserPathForArgs { get; set; }

        /// <summary>
        /// Прямой путь к папке для удаления сгенерированных временных пользователей LibreOffice.
        /// </summary>
        public string TempUserDirectPathForDelete { get; set; }

        /// <summary>
        /// Максимальное количество параллельно запущенных экземпляров LibreOffice.
        /// </summary>
        public int MaxConcurrentLibreOfficeInstances { get; set; } = 1;
    }
}
