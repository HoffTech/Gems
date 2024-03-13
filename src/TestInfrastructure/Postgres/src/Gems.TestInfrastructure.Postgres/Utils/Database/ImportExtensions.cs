using System.Text;

using Csv;

using Gems.TestInfrastructure.Utils.Database;
using Gems.TestInfrastructure.Utils.Database.Import;
using Gems.TestInfrastructure.Utils.Database.Import.Csv;

using Npgsql;

namespace Gems.TestInfrastructure.Postgres.Utils.Database
{
    public static class ImportExtensions
    {
        public static async Task ImportAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            IAsyncEnumerable<IImportRow> rows,
            CancellationToken cancellationToken = default)
        {
            await ImportHelper.ImportAsync(
                connection,
                table,
                rows,
                ":",
                () => new NpgsqlCommand(),
                (name, value) => new NpgsqlParameter(name, value),
                cancellationToken);
        }

        public static async Task ImportCsvAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            Stream stream,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            var optionsBuilder = new CsvOptionsBuilder();
            configure?.Invoke(optionsBuilder);

            await ImportAsync(
                connection,
                table,
                ImportRowAdapter.ConvertToImportRows(
                    CsvReader.ReadFromStreamAsync(stream, optionsBuilder.Options)),
                cancellationToken);
        }

        public static async Task ImportCsvAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            TextReader reader,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            var optionsBuilder = new CsvOptionsBuilder();
            configure?.Invoke(optionsBuilder);

            await ImportAsync(
                connection,
                table,
                ImportRowAdapter.ConvertToImportRows(
                    CsvReader.ReadAsync(reader, optionsBuilder.Options)),
                cancellationToken);
        }

        public static async Task ImportCsvFileAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            string fileName,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            await ImportCsvAsync(
                connection,
                table,
                File.OpenText(fileName),
                configure,
                cancellationToken);
        }

        public static async Task ImportCsvFileAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            FileInfo fileInfo,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            await ImportCsvAsync(
                connection,
                table,
                fileInfo.OpenText(),
                configure,
                cancellationToken);
        }

        public static async Task ImportCsvFileAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            string fileName,
            Encoding encoding,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            await ImportCsvAsync(
                connection,
                table,
                new StreamReader(File.OpenRead(fileName), encoding),
                configure,
                cancellationToken);
        }

        public static async Task ImportCsvFileAsync(
            this NpgsqlConnection connection,
            TableMetadata table,
            FileInfo fileInfo,
            Encoding encoding,
            Action<ICsvOptionsBuilder> configure = null,
            CancellationToken cancellationToken = default)
        {
            await ImportCsvAsync(
                connection,
                table,
                new StreamReader(fileInfo.OpenRead(), encoding),
                configure,
                cancellationToken);
        }
    }
}
