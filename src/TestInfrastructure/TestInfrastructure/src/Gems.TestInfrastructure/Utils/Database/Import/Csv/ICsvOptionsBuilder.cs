// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database.Import.Csv
{
    public interface ICsvOptionsBuilder
    {
        ICsvOptionsBuilder EnableTrimData(bool enable);

        ICsvOptionsBuilder Skip(int rowsSkip);

        ICsvOptionsBuilder UseConverter(Func<string, string, object> converter);

        ICsvOptionsBuilder UseFilter(Func<ReadOnlyMemory<char>, int, bool> filter);

        ICsvOptionsBuilder UseHeaderComparer(IEqualityComparer<string> equalityComparer);

        ICsvOptionsBuilder UseNewLine(string newLine);

        ICsvOptionsBuilder UseSeparator(char separator);
    }
}
