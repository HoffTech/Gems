# Импорт из CSV

При тестировании может понадобиться импортировать данные из CSV.
Для этого можно использовать методы расширения *Gems.Test*. 
Пример:

```csharp
await using var env = await new TestEnvironmentBuilder()
    .UsePostgres(
        "DefaultConnection",
        async (c, ct) =>
        {
            await c.ExecScriptAsync(new FileInfo(@"Resources/Sql/migration.sql"), ct);
            await c.SetupAsync(
                async (connection, schema) =>
                {
                    await connection.ImportCsvFileAsync(
                        schema.CurrentDatabase.Table("precipitations"),
                        new FileInfo(@"Resources/Csv/precipitations.csv"));
                },
                ct);
        })
    .BuildAsync();
```

Импорт возможен в базы данных Postgresql и MsSql.

## Postgresql

```csharp
public static async Task ImportCsvFileAsync(
    this NpgsqlConnection connection,
    TableMetadata table,
    string fileName,
    Encoding encoding,
    Action<ICsvOptionsBuilder> configure = null,
    CancellationToken ct = default)
```

Импортирует файл CSV в таблицу базы данных

- `connection` - соединение с базой данных,
- `table` - целевая таблица
- `fileName` - файл CSV
- `encoding` - кодировка исходного файла CSV
- `configure` - делегат для настройки CSV

## MsSql

```csharp
public static async Task ImportCsvFileAsync(
    this SqlConnection connection,
    TableMetadata table,
    string fileName,
    Encoding encoding,
    Action<ICsvOptionsBuilder> configure = null,
    CancellationToken ct = default)
```

Импортирует файл CSV в таблицу базы данных

- `connection` - соединение с базой данных,
- `table` - целевая таблица
- `fileName` - файл CSV
- `encoding` - кодировка исходного файла CSV
- `configure` - делегат для настройки CSV
