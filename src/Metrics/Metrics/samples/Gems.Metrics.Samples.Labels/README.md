# Работа с лейблами

> **Внимание:** использование большого количества лейблов - плохая практика, это повышает кардинальность метрики и влияет на производительность отображения графиков в Graphana

**Для того чтобы использовать лейблы  необходимо:**
1. В классе `Startup.cs` подключите маппинг метрик
    ```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }
    ```

2. Создайте перечисление `CreatePersonMetricType` для хранения метрики
    ```csharp
    public enum ImportPersonsMetricType
    {
        [Metric(
            Name = "import_persons",
            Description = "Импорт персон",
            LabelNames = new[] { "operation_type" })]
        ImportPersonCounters
    }
    ```
3. Вызовите метод установки _Gauge_ метрики, при этом используйте перегрузку с указанием параметра `params LabelValues`
    ```csharp
        public async Task Handle(ImportPersonsCommand command, CancellationToken cancellationToken)
        {
            // Получение количества добавленных, обновленных и удаленных персон после выполнения функции импорта
            var counters = this.ImportPersons();

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Added, labelValues: "add")
                .ConfigureAwait(false);

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Updated, labelValues: "update")
                .ConfigureAwait(false);

            await metricsService
                .GaugeSet(ImportPersonsMetricType.ImportPersonCounters, counters.Deleted, labelValues: "delete")
                .ConfigureAwait(false);
        }
    ```
4. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрики
    ```
    # HELP import_persons Импорт персон
    # TYPE import_persons gauge
    import_persons{operation_type="delete"} 300
    import_persons{operation_type="add"} 100
    import_persons{operation_type="update"} 200
    ```


### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
