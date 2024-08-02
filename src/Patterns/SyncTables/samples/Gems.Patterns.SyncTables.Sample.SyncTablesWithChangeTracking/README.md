# Пример синхронизации данных с использованием ChangeTracking

Этот пример демонстрирует как использовать типовое решение для задач синхронизации таблиц из базы данных источника в целевую базу данных с использованием ChangeTracking.

**Данный пример не для запуска, так как нужны дополнительные настройки в виде:**
1. Строк подключения к базе данных источника.
2. Миграции базы данных источника: **[Пример Sql миграции](/src/Patterns/SyncTables/samples/Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking/Migrations/Source)**
3. Строк подключения к целевой базе данных.
4. Миграции целевой бд **[Пример Sql миграции](/src/Patterns/SyncTables/samples/Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking/Migrations/Destination)**
После данных действий пример готов к использованию, для активации синхронизации нужно лишь вызвать метод api.

Для добавления синхронизации таблицы [dbo].[Person] необходимо добавить фичу SyncPersons, а вней:
1. В файле SyncPersonsServicesConfiguration.cs сконфигурировать опции и синхронизатор. (AddChangeTrackingTableSyncer можно добавлять в Startup.cs)

```csharp
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SyncPersonsInfoOptions>(configuration.GetSection(SyncPersonsInfoOptions.SectionName));
        services.AddChangeTrackingTableSyncer(configuration.GetSection(ImportPersonsFromDaxOptions.SectionName));
    }
```
2. Настроить мепинг/преобразование сущностей 
```csharp
    public SyncPersonsMappingProfile()
    {
        this.CreateMap<ExternalPerson, Person>();
    }
```

3. Добавить конфигурацию функций сохранения информации по ChangeTracking в appsettings.json:

```json
    "UpsertVersionFunctionInfo": {
      "FunctionName": "public.sync_info_upsert_by_table_name",
      "TableParameterName": "p_table_name",
      "RowVersionParameterName": "p_row_version"
    },
    "ProviderVersionFunctionInfo": {
      "FunctionName": "public.sync_info_get_last_for_table",
      "TableParameterName": "p_table_name"
    }
```
`UpsertVersionFunctionInfo` задаёт настройки функции в базе данных для обновления RV
`ProviderVersionFunctionInfo` задаёт настройки функции в базе данных для получения RV

4. Добавьте код запускающий синхронизацию, так где это необходимо:
```csharp 
[Endpoint("api/v1/sync-persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler(
    IChangeTrackingSyncTableProcessor<ExternalPerson, Person, RowCounters> changeTrackingProcessor,
    IOptions<SyncPersonsInfoOptions> options)
    : IRequestHandler<SyncPersonsCommand, List<RowCounters>>
{
    public async Task<List<RowCounters>> Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        var syncResult = await changeTrackingProcessor.Sync(
            new ChangeTrackingSyncInfo(
                new SourceDataSettings
                {
                    DbKey = "source",
                    TableName = "dbo.Person",
                    GetCommandTimeout = options.Value.GetPersonsInfoTimeout,
                    BatchSize = 100_000,
                    PrimaryKeyName = "RecId",
                    ChangesQuery =
                        """
                        """                    
                    FullReloadQuery =
                        """
                        """,
                    OnRestoreFromBackupDetected = SyncErrorAction.Fail,
                    OnDestinationVersionOutdated = SyncErrorAction.Fail
                },
                new DestinationSettings
                {
                    DbKey = "destination",
                    TableName = "public.person",
                    MergeFunctionName = "public.person_merge",
                    MergeParameterName = "p_changed_data",
                    EnableFullChangesLog = false,
                    ClearFunctionName = "public.person_clear"
                },
                true),
            cancellationToken);

        return syncResult.MergeResults.ToList();
    }
}
```
Следует обратить внимание на настройки поведения при обнаружении ситуаций, в которых дальнейшая синхронизация невозможна, например:
- база данных источник восстановлена из бэкапа
- целевая база данных отстала в синхронизации больше чем на период синхронизации

в данных случаях возможно выбрать из следующих вариантов поведения:
```csharp
    Log,
    Fail,
    FullReload
```

При указании запроса для полной синхронизации в запросе можно указать параметры @offset и @batchSize, в этом случае будет использован пакетный режим
```sql
    where
        [RecId] >= @offset and [RecId] < @offset + @batchSize
```
Запроса изменений по ChangeTracking имеет ряд особенностей 
```sql
    WITH ChangedPersonCTE
    (
        [ChangeTrackingVersion],
        [OperationType]
        --skipped
    )
    AS
    (
        SELECT
            ct.SYS_CHANGE_VERSION [ChangeTrackingVersion],
            ct.SYS_CHANGE_OPERATION [OperationType]
            --skipped
        FROM changetable(changes dbo.[Person], @version) as ct
        LEFT JOIN dbo.[Person] as it
            on ct.RECID = it.RECID
    )

    SELECT TOP (@batchSize) WITH TIES
        *
    FROM  ChangedPersonCTE WITH (FORCESEEK)
    ORDER BY ChangeTrackingVersion
    OPTION(MAXDOP 1)    
```
1. Сам запрос данных changetable обернут в CTE, для того, что бы указать sql серверу необходимость использовать seek по ситсемному индексу CT
2. TOP (@batchSize) WITH TIES + ORDER BY ChangeTrackingVersion обеспечивает "гибкий" batching - запрос стремится возвращать @batchSize записей, но при этом никогда не выдаст часть данных по одной транзакции


Для каждой синхронизации автоматически добавляются метрики по всем стадиям: 
- время загрузки данных из источника (гистограмма)
- время преобразования данных (гистограмма)
- время сохранения данных в целевой бд (гистограмма)
- количество данных (гистограмма)
