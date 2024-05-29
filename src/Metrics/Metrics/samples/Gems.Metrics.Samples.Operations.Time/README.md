# Установка Time метрики

**Для того чтобы использовать метрику типа _Time_ необходимо:**
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
            Name = "persons_registration_time",
            Description = "Время регистрации персоны")]
        PersonRegistrationTime
    }
    ```
3. Вызовите метод установки _Time_ метрики
    ```csharp
    private async Task<PersonDto> RegisterPersonAsync(PersonDto person, CancellationToken cancellationToken)
    {
        // установка временной метрики на длительность процесса регистрации персоны
        await using var timeMetric = metricsService
            .Time(CreatePersonMetricType.PersonRegistrationTime)
            .ConfigureAwait(false);

        // мнимый процесс регистрации
        await Task.Delay(1500, cancellationToken).ConfigureAwait(false);

        return person;
    }
    ```
4. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик в миллисекунлах
    ```
    # HELP persons_registration_time Время регистрации персоны
    # TYPE persons_registration_time gauge
    persons_registration_time 1525.8782
    ```


### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
