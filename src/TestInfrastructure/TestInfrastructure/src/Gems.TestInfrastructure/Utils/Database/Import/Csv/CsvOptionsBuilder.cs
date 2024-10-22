// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Csv;

namespace Gems.TestInfrastructure.Utils.Database.Import.Csv
{
    public class CsvOptionsBuilder : ICsvOptionsBuilder
    {
        private readonly CsvOptions options = new CsvOptions();
        private Func<string, string, object> converter;

        public CsvOptionsBuilder()
        {
            this.options.AllowNewLineInEnclosedFieldValues = true;
            this.options.HeaderMode = HeaderMode.HeaderPresent;
        }

        public CsvOptions Options => this.options;

        public Func<string, string, object> Converter => this.converter;

        public ICsvOptionsBuilder UseConverter(Func<string, string, object> converter)
        {
            this.converter = converter;
            return this;
        }

        public ICsvOptionsBuilder Skip(int rowsSkip)
        {
            this.Options.RowsToSkip = rowsSkip;
            return this;
        }

        public ICsvOptionsBuilder UseFilter(Func<ReadOnlyMemory<char>, int, bool> filter)
        {
            this.Options.SkipRow = filter;
            return this;
        }

        public ICsvOptionsBuilder UseSeparator(char separator)
        {
            this.Options.Separator = separator;
            return this;
        }

        public ICsvOptionsBuilder EnableTrimData(bool enable)
        {
            this.Options.TrimData = enable;
            return this;
        }

        public ICsvOptionsBuilder UseHeaderComparer(IEqualityComparer<string> equalityComparer)
        {
            this.Options.Comparer = equalityComparer;
            return this;
        }

        public ICsvOptionsBuilder UseNewLine(string newLine)
        {
            this.Options.NewLine = newLine;
            return this;
        }
    }
}
