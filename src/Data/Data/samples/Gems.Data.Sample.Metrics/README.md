# Метрики выполнения операций в БД

### Основные понятия
- Интерфейс _IUnitOfWork_ предлагает список операций, которые можно выполнить в БД. По окончанию выполнения операции в Prometheus записывается метрика времени выполнения той или иной операции
- Запись метрик для операций _IUnitOfWork_ реализована в библиотеке [Gems.Metrics.Data](/src/Metrics/Data/README.md#метрики-с-iunitofwork)
- Метрика устанавливается по умолчанию, но при необходимости ее можно переопределить в конфигурации

### Как работать с метриками
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
```
2) Зарегистрируйте _middleware_ для маппинга метрик _Prometheus_ и доступа к странице `/metrics`
```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics(); // Маппинг метрик
        });
    }
```
3) Инъектируйте в конструктор класса интерфейс **IUnitOfWorkProvider**
```csharp
    private readonly IUnitOfWorkProvider unitOfWorkProvider;
    
    public CreatePersonCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }
```

4) Получите объект **UnitOfWork** из провайдера **IUnitOfWorkProvider** и вызовите одну из доступных операций. В примере ниже вызываетяся функция получения объекта _Person_ по _id_
```csharp
        public async Task<PersonDto> Handle(GetPersonQuery query, CancellationToken cancellationToken)
        {
            var person = await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .CallTableFunctionFirstAsync<Person>(
                    "public.person_get_person_by_id",
                    new Dictionary<string, object>
                    {
                        ["p_person_id"] = query.Id
                    });

            return this.mapper.Map<PersonDto>(person);
        }
```
5) Перейдите на страницу с метриками `http(s)://<your_domain>:<your_port>/metrics` для снятия результатов записи метрик

### Метрика по умолчанию
> По умолчанию конфигурация метрик не требуется. Записывается метрика по умолчанию.

**Пример**:
```
# HELP db_query_time Db Query Time
# TYPE db_query_time gauge
db_query_time{functionName="public_person_get_person_by_id"} 385.7019
```
- `db_query_time` - наименование метрики
- `functionName` - наименование функции/процедуры в БД
- `public_person_get_person_by_id` - значение функции/процедуры в БД (символы `.` заменены на `_`)
- `385.7019` - время выполнения функции/процедуры

### Переопределение глобальной конфигурации
Для того чтобы переопределить глобальное имя метрики для _UnitOfWork_:
1. Зарегистрироуйте _UnitOfWork_ с переопределенной конфигурацией. Есть 2 способа конфигурации: В appsettings.json или в Startup.cs
   1. В `appsettings.json`
       ```json
       "PostgresqlUnitOfWorks": [
        {
          "Key": "default",
          "Options": {
            "ConnectionString": "${ConnectionStrings.DefaultConnection}",
            "DbQueryMetricInfo": {
              "Name": "gems_data_sample_metrics_db_query",
              "Description": "Time of Gems Data Sample Metrics Db Query",
              "LabelNames": ["query_name"]
            }
          }
        }
      ]
        ```
   2. В `Startup.cs`
      1. Создайте перечисление
        ```csharp
        public enum DbQueryMetricType
        {
            // Применение атрибута опционально, если необходимо переопределить дополнительные поля
            [Metric(
                Name = "gems_data_sample_metrics_db_query",
                Description = "Time of Gems Data Sample Metrics Db Query",
                LabelNames = new[] { "query_name" })]
            GemsDataSampleMetricsDbQueryTime
        }
        ```
      2. Зарегистрируйте перечисление в `Startup.cs`
        ```csharp
        opt.AddUnitOfWorks = () =>
        {
            services.AddPostgresqlUnitOfWork(
                "default",
                options =>
                {
                    options.DbQueryMetricType = DbQueryMetricType.GemsDataSampleMetricsDbQueryTime;
        
                    // Альтернативная конфигурация без использования перечисления
                    options.DbQueryMetricInfo = new MetricInfo { Name = "gems_data_sample_metrics_db_query_time", Description = "Gems Data Sample Metrics Db Query Time" };
                });
        };
        ```
3. Зафиксируйте результат на странице `/metrics`
```
# HELP gems_data_sample_metrics_db_query Time of GemsDataSampleMetrics db query
# TYPE gems_data_sample_metrics_db_query gauge
gems_data_sample_metrics_db_query{query_name="public_person_get_person_by_id"} 370.6968
```

### Переопределение метрики атомарно для операции
Для того чтобы переопределить метрику для отдельно взятой операции в БД
1. Создайте перечисление
```csharp
public enum AtomicQueryMetricType
{
    [Metric(
        Name = "gems_data_sample_atomic_metrics_db_query",
        Description = "Time of Atomic Gems Data Sample Metrics db query"
    GemsDataSampleMetricsAtomicDbQueryTime
}
```
2. Передайте перечисление из п.1 на вход в параметры операции (в примере ниже _CallScalarFunctionAsync_)
```csharp
return this.unitOfWorkProvider
    .GetUnitOfWork(cancellationToken)
    .CallScalarFunctionAsync<int>(
        "public.person_get_age_by_id",
        new Dictionary<string, object>
        {
            ["p_person_id"] = query.Id
        },
        AtomicQueryMetricType.GemsDataSampleMetricsAtomicDbQueryTime);
```
3. Зафиксируйте результат на странице `/metrics`
```
# HELP gems_data_sample_atomic_metrics_db_query Time of Atomic Gems Data Sample Metrics db query
# TYPE gems_data_sample_atomic_metrics_db_query gauge
gems_data_sample_atomic_metrics_db_query 325.6174
```

### Запуск примера
1. Для проверки сэмпла, настройте подключение к реальной БД
2. Реализуйте функции/процедуры
   1. Получение объекта **Person** по id `public.person_get_person_by_id`
   2. Получение возраста объекта **Person** по id `public.person_get_age_by_id`
4. Вызовите ендпоинты с помощью **Swagger**
    1. `GET /api/v1/persons/{id}`
    2. `GET /api/v1/persons/{id}/age`
