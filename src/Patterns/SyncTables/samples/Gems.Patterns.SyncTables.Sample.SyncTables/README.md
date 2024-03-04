# Пример синхронизации данных без использования ChangeTracking

Этот пример демонстрирует как использовать типовое решение для задач синхронизации таблиц из базы данных источника в целевую базу данных без использованием ChangeTracking.

**Данный пример не для запуска, так как нужны дополнительные настройки в виде:**
1. Строк подключения к базе данных источника.
2. Строк подключения к целевой базе данных.
3. Настройки запросов к базе данных источника.

Добавьте следующую строку в конфигурацию сервиса: 

```csharp
services.AddTableSyncer();
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
И затем передать объявленное перечисление в параметр Enum externalDbQueryMetricType конструктора класса **MergeInfo**

С помощью `MergeProcessorFactory` вызовите метод `CreateMergeProcessor`, который создаст сам процессор и затем у него метод `ProcessCollectionAsync`.
Если результат операции не нужен, как в данном примере, то можно использовать `EmptyMergeResult` 

```csharp
[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class SyncPersonsCommandHandler : IRequestHandler<SyncPersonsCommand>
{
    private readonly MergeProcessorFactory mergeProcessorFactory;

    public SyncPersonsCommandHandler(MergeProcessorFactory mergeProcessorFactory)
    {
        this.mergeProcessorFactory = mergeProcessorFactory;
    }

    public Task Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        return new MergeCollection<EmptyMergeResult>(
            new List<BaseMergeProcessor<EmptyMergeResult>>
            {
                this.mergeProcessorFactory.CreateMergeProcessor<ExternalPerson, Person, EmptyMergeResult>(
                    new MergeInfo<EmptyMergeResult>(
                        sourceDbKey: "Dax",
                        externalSyncQuery: "SELECT * FROM Persons",
                        mergeFunctionName: "common.persons_upsert",
                        mergeParameterName: "p_persons",
                        needConvertDateTimeToUtc: false,
                        getCommandTimeout: 0,
                        targetDbKey: "DefaultConnection",
                        externalDbQueryMetricType: SyncPersonsMetricType.GetExternalPersonEntities))
            })
            .ProcessCollectionAsync(cancellationToken);
    }
}
```