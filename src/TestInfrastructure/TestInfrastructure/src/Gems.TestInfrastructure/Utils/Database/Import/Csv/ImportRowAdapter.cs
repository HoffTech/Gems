// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Csv;

namespace Gems.TestInfrastructure.Utils.Database.Import.Csv
{
    public class ImportRowAdapter : IImportRow
    {
        private readonly Func<string, string, object> converter;

        public ImportRowAdapter()
        {
        }

        public ImportRowAdapter(ICsvLine line, Func<string, string, object> converter)
        {
            this.Line = line;
            this.converter = converter;
        }

        public ICsvLine Line { get; set; }

        public static async IAsyncEnumerable<IImportRow> ConvertToImportRows(IAsyncEnumerable<ICsvLine> lines)
        {
            var adapter = new ImportRowAdapter();
            await foreach (var line in lines)
            {
                adapter.Line = line;
                yield return adapter;
            }
        }

        public string[] GetColumns()
        {
            return this.Line.Headers;
        }

        public object GetValue(int i)
        {
            var strValue = this.Line[i];
            if (this.converter != null)
            {
                var header = this.Line.Headers[i];
                return this.converter(strValue, header);
            }
            else
            {
                return strValue;
            }
        }
    }
}
