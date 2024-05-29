# Сброс метрик

**Для того чтобы настроить сброс метрик необходимо:**
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

2. В `appsettings.json` укажите конфигурацию сброса _(без указания явной настройки, сброс по умолчанию равен 60000 миллисекунд)_
```json
  "MetricsConfig": {
    "ResetMillisecondsDelay": 60000 // время, через которое необходимо сбрасывать метрики, по умолчанию 60 секунд
  }
```

3. Создайте произвольное перечисление, например `CreatePersonMetricType` для хранения метрики
    ```csharp
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "persons_age",
            Description = "Возраст персоны")]
        PersonAge
    }
    ```

4. Вызовите метод установки _Gauge_ метрики
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
5. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрики

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
