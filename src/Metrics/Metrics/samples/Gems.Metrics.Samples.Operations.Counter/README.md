# Установка Counter метрики

**Для того чтобы использовать метрику типа _Counter_ необходимо:**
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
            Name = "persons_created",
            Description = "Количество созданных персон")]
        PersonsCreated
    }
    ```
3. Вызовите метод инкремента _Counter_ метрики _(в данном случае при каждом вызове метода счетчик инкрементируется на значение 1 с момента старта сервиса)_
    ```csharp
    public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        // ...
        
        await metricsService.Counter(CreatePersonMetricType.PersonsCreated, increment: 1).ConfigureAwait(false);
    
        // ...
    }
    ```
4. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
    ```
    # HELP persons_created Количество созданных персон
    # TYPE persons_created counter
    persons_created 3
    ```


### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
