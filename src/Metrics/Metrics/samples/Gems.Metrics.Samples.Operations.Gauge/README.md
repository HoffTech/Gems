# Установка Gauge метрики

**Для того чтобы использовать метрику типа _Gauge_ необходимо:**
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
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "persons_age",
            Description = "Возраст персоны")]
        PersonAge
    }
    ```
3. Вызовите метод установки _Gauge_ метрики
    ```csharp
    public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        // ...
        
        // По выборке данной метрики можно отслеживать возраст передаваемых персон и вычислить средний возраст
        await metricsService
            .GaugeSet(CreatePersonMetricType.PersonsCreated, targetValue: command.Age)
            .ConfigureAwait(false);
    
        // ...
    }
    ```
4. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрики
    ```
    # HELP persons_age Возраст персоны
    # TYPE persons_age gauge
    persons_age 30
    ```


### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
