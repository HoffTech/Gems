# Пример синхронизации данных с использованием ChangeTracking

Этот пример демонстрирует как использовать типовое решение для задач синхронизации таблиц из базы данных источника в целевую базу данных с использованием ChangeTracking.

**Данный пример не для запуска, так как нужны дополнительные настройки в виде:**
1. Строк подключения к базе данных источника.
2. Строк подключения к целевой базе данных.
3. Настройки запросов к базе данных источника.
4. Конфигурирование секции `ImportPersonsFromDax` и `ProviderVersionFunctionInfo`

Добавьте следующую строку в конфигурацию сервиса: 

```csharp
services.AddTableSyncer(configuration.GetSection(SyncTablesOptions.SectionName));
```

Добавьте конфигурацию в файле appsettings.json:

```json
  "Person": {
    "ImportPersonsFromDax": {      
      "UpsertVersionFunctionInfo": {
        "FunctionName": "public.import_rv_upsertrowversionbytablename",
        "TableParameterName": "p_table_name",
        "RowVersionParameterName": "p_row_version"
      },
      "ProviderVersionFunctionInfo": {
        "FunctionName": "public.import_rv_getlastrowversionfortable",
        "TableParameterName": "p_table_name"
      }
    }
  }
```
`UpsertVersionFunctionInfo` задаёт настройки функции в базе данных для обновления RV
`ProviderVersionFunctionInfo` задаёт настройки функции в базе данных для получения RV

Для кастомизации свойств, можно унаследоваться от класса ChangeTrackingSyncOptions и зарегистрировать свою секцию:
```csharp
public class ImportPersonsFromDaxOptions : ChangeTrackingSyncOptions
{
    /// <summary>
    /// Section in appsettings.json.
    /// </summary>
    public const string SectionName = "Person:ImportPersonsFromDax";
}
```

Опционально есть возможность подключения метрик для запросов в базу данных источника, для этого нужно создать перечисления, включающего данные по метрикам:

```csharp
public enum SyncPersonsMetricType
{
    [Metric(
        Name = "mssql_db_query_time",
        LabelNames = new[] { "query_name" },
        LabelValues = new[] { "get_external_person_entities" })]
    GetExternalPersonEntities
}
```
И затем передать объявленное перечисление в параметр Enum externalDbQueryMetricType конструктора класса **ChangeTrackingMergeInfo**

С помощью `ChangeTrackingMergeProcessorFactory` вызовите метод `CreateChangeTrackingMergeProcessor`, который создаст сам процессор и затем у него метод `ProcessCollectionAsync`.
Результатом работы метода является массив `RowCounters` содержащий информацию об результате операции.
Если результат операции не нужен, то можно использовать `EmptyMergeResult` вместо `RowCounters` 

```csharp
[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler : IRequestHandler<SyncPersonsCommand, List<RowCounters>>
{
    private readonly ChangeTrackingMergeProcessorFactory processorFactory;

    private readonly IOptions<SyncPersonsInfoOptions> options;

    public SyncPersonsCommandHandler(
        ChangeTrackingMergeProcessorFactory processorFactory,
        IOptions<SyncPersonsInfoOptions> options)
    {
        this.processorFactory = processorFactory;
        this.options = options;
    }

    public Task<List<RowCounters>> Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        return new MergeCollection<RowCounters>(
            new List<BaseMergeProcessor<RowCounters>>
            {
                this.processorFactory.CreateChangeTrackingMergeProcessor<ExternalPerson, Person, RowCounters>(
                    new ChangeTrackingMergeInfo<RowCounters>(
                        sourceDbKey: "Dax",
                        tableName: "Persons",
                        externalSyncQuery: "SELECT * FROM Persons",
                        mergeFunctionName: "public.persons_mergepersons",
                        mergeParameterName: "p_persons",
                        needConvertDateTimeToUtc: true,
                        getCommandTimeout: this.options.Value.GetPersonsInfoTimeout,
                        targetDbKey: "DefaultConnection",
                        SyncPersonsMetricType.GetExternalPersonEntities))
            }).ProcessCollectionAsync(cancellationToken);
    }
}
```