# Сброс метрик

**Для того чтобы настроить сброс метрик необходимо:**

> По умолчанию сброс происходит через **60 секунд**
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

2. Создайте произвольное перечисление, например `CreatePersonMetricType` для хранения метрики
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
        // установка Gauge метрики
        await metricsService
            .GaugeSet(CreatePersonMetricType.PersonsCreated, targetValue: command.Age)
            .ConfigureAwait(false);
        // ...
    }
    ```
4. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрики

    1. Запись метрики
        ```
        # HELP persons_age Возраст персоны
        # TYPE persons_age gauge
        persons_age 30
        ```
   2. Сброс метрики спустя `600000` миллисекунд
        ```
        # HELP persons_age Возраст персоны
        # TYPE persons_age gauge
        persons_age 0
        ```

### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
