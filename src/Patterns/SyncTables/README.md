# Gems.Patterns.SyncTables

Реализация паттернов синхронизации таблиц

# Содержание

* [Установка](#установка)
* [Конфигурация](#конфигурация)
* [Описание](#описание)
* [Примеры](#примеры)

# Установка
Установите nuget пакет Gems.Patterns.SyncTables через менеджер пакетов

# Конфигурация
1. Добавьте следующую строку в конфигурацию сервиса:
```csharp
services.AddTableSyncer(configuration.GetSection(nameof(SyncTablesOptions)));
```
2. Оционально (Если для синхронизации используется механизм ChangeTracking):
   *  Добавьте конфигурацию в файле appsettings.json:
    ```csharp
    "ChangeTrackingSyncOptions": {
      "UpsertVersionFunctionInfo": {
        "FunctionName": "public.import_rv_upsertrowversionbytablename",
        "TableParameterName": "p_table_name",
        "RowVersionParameterName": "p_row_version"
      },
      "ProviderVersionFunctionInfo": {
        "FunctionName": "public.import_rv_getlastrowversionfortable",
        "TableParameterName": "p_table_name"
      }
    ```
   * Для кастомизации свойств, можно унаследоваться от класса ChangeTrackingSyncOptions и зарегистрировать свою секцию
3. Опционально (Возможность подключения метрик для запросов в БД источник)
    * Создайте перечисления, включающие данные по метрикам
    ```csharp
   public enum SyncServiceOrderDbQueryMetricType
   {
        [Metric(
            Name = "mssql_db_query_time",
            LabelNames = new[] { "query_name" },
            LabelValues = new[] { "get_external_entities" })]
        GetExternalEntities
   }
    ```
   * Передайте объявленное перечисление в параметр **Enum externalDbQueryMetricType** конструктора класса **ChangeTrackingMergeInfo** или **MergeInfo**

# Описание
Библиотека содержит в себе типовые решения для задач синронизации таблиц из БД источника в целевую БД и включает 2 имплементации синхронизации данных:
- c использованием ChangeTracking
- без использования ChangeTracking

Храимые процедуры логики синхронизации реализуются индивидуально в проекте сервиса для каждого объекта `MergeInfo<TMergeResult>` 

# Примеры
* Пример клиента, работающего с ChangeTracking:
```csharp
    public class ChangeTrackingMergeClient
    {
        private readonly ChangeTrackingMergeProcessorFactory processorFactory;

        public ChangeTrackingMergeClient(ChangeTrackingMergeProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public async Task<List<MergeResult>> ProcessMergesAsync(
            ChangeTrackingMergeInfo<MergeResult> mergeInfo1,
            ChangeTrackingMergeInfo<MergeResult> mergeInfo2,
            CancellationToken cancellationToken)
        {
            var mergeCollection = new MergeCollection<MergeResult>(new List<BaseMergeProcessor<MergeResult>>
            {
                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<ExternalEntity, TargetEntity, MergeResult>(
                        mergeInfo1),

                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<ExternalEntity, TargetEntity, MergeResult>(
                        mergeInfo2),
            });

            return await mergeCollection.ProcessCollectionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
```
* Пример клиента, работающего без ChangeTracking:
```csharp
    public class MergeClient
    {
        private readonly MergeProcessorFactory processorFactory;

        public MergeClient(MergeProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public async Task<List<MergeResult>> ProcessMergesAsync(
            MergeInfo<MergeResult> mergeInfo1,
            MergeInfo<MergeResult> mergeInfo2,
            MergeInfo<MergeResult> mergeInfo3,
            CancellationToken cancellationToken)
        {
            var mergeCollection = new MergeCollection<MergeResult>(new List<BaseMergeProcessor<MergeResult>>
            {
                this.processorFactory.CreateMergeProcessor<ExternalEntity, TargetEntity, MergeResult>(mergeInfo1),
                this.processorFactory.CreateMergeProcessor<ExternalEntity, TargetEntity, MergeResult>(mergeInfo2),
                this.processorFactory.CreateMergeProcessor<ExternalEntity, TargetEntity, MergeResult>(mergeInfo3)
            });

            return await mergeCollection.ProcessCollectionAsync(cancellationToken).ConfigureAwait(false);
        }
    }
```

* Пример клиента, работающего с ChangeTracking и не возвращающего резуьтат:
```csharp
    public class ChangeTrackingMergeClientWithEmptyResult
    {
        private readonly ChangeTrackingMergeProcessorFactory processorFactory;

        public ChangeTrackingMergeClient(ChangeTrackingMergeProcessorFactory processorFactory)
        {
            this.processorFactory = processorFactory;
        }

        public async Task<List<EmptyMergeResult>> ProcessMergesAsync(
            ChangeTrackingMergeInfo<EmptyMergeResult> mergeInfo1,
            ChangeTrackingMergeInfo<EmptyMergeResult> mergeInfo2,
            CancellationToken cancellationToken)
        {
            var mergeCollection = new MergeCollection<MergeResult>(new List<BaseMergeProcessor<EmptyMergeResult>>
            {
                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<ExternalEntity, TargetEntity, EmptyMergeResult>(
                        mergeInfo1),

                this.processorFactory
                    .CreateChangeTrackingMergeProcessor<ExternalEntity, TargetEntity, EmptyMergeResult>(
                        mergeInfo2),
            });

            return await mergeCollection.ProcessCollectionAsync(cancellationToken).ConfigureAwait(false);
        }
```

Метод запуска процесса синхронизации `ProcessMergesAsync` принимает на вход параметры `MergeInfo<MergeResult>` для каждого отдельного клиента количество параметров может быть разным. Внутри метода посредством фабрики создаются экземлляры процессоров, которые будут отвечать за обработку отдельно взятых объектов `MergeInfo<TMergeResult>`, типизация для каждого процессора настраивается индивидуально.

* Пример вызова метода `ProcessMergesAsync` c использованием ChangeTracking:
```csharp
var result = await client.ProcessMergesAsync(
        new ChangeTrackingMergeInfo<ExternalEntity, MergeResult>(
            sourceDbKey: "axapta",
            targetDbKey: "default",
            tableName: "sync_table_1",
            externalSyncQuery: "external_query_1",
            mergeFunctionName: "public.merge_function_1",
            mergeParameterName: "p_entities",
            needConvertDateTimeToUtc: true),
        new ChangeTrackingMergeInfo<ExternalEntity, MergeResult>(
            sourceDbKey: "axapta",
            targetDbKey: "default",
            tableName: "sync_table_2",
            externalSyncQuery: "external_query_2",
            mergeFunctionName: "public.merge_function_2",
            mergeParameterName: "p_entities",
            needConvertDateTimeToUtc: false),
        CancellationToken.None)
    .ConfigureAwait(false);
```
* Пример вызова метода `ProcessMergesAsync` без использования ChangeTracking:
```csharp
 var result = await client.ProcessMergesAsync(
        new MergeInfo<ExternalEntity, MergeResult>(
            sourceDbKey: "axapta",
            targetDbKey: "default",
            externalSyncQuery: "external_query_1",
            mergeFunctionName: "public.merge_function_1",
            mergeParameterName: "p_entities",
            needConvertDateTimeToUtc: true),
        new MergeInfo<ExternalEntity, MergeResult>(
            sourceDbKey: "axapta",
            targetDbKey: "default",
            externalSyncQuery: "external_query_2",
            mergeFunctionName: "public.merge_function_2",
            mergeParameterName: "p_entities",
            needConvertDateTimeToUtc: false)
    .ConfigureAwait(false);

```