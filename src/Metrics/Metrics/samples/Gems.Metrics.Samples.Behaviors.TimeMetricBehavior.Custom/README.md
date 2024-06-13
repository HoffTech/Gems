# Использование пайплайна TimeMetricBehavior с переопределением метрики

**Для того чтобы использовать пайплайн _TimeMetricBehavior_ с переопределением метрики по умолчанию необходимо:**
1. В классе `Startup.cs` подключите маппинг метрик и зарегистрируйте пайплайн _TimeMetricBehavior_ (регистрируется по умолчанию в Gems.CompositionRoot)
    ```csharp
   public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(
            configuration,
            opt =>
            {
                // регистрируется по умолчанию в Gems.CompositionRoot 
                opt.AddPipelines = () => services.AddPipeline(typeof(TimeMetricBehavior<,>));
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }
    ```

2. Создайте перечисление с метрикой для переопределения
    ```csharp
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "create_person_custom_time_metric",
            Description = "Создание персоны уникальная пользовательская временная метрика")]
        CreatePersonTime
    }
    ```

3. Унаследуйте команду `CreatePersonCommand` от интерфейса `IRequestTimeMetric` и переопределите метод `public Enum GetTimeMetricType()`, возвратив метрику созданную в п.2
    ```csharp
    public record CreatePersonCommand : IRequest<PersonDto>, IRequestTimeMetric
    {
        // ...

        public Enum GetTimeMetricType()
        {
            return CreatePersonMetricType.CreatePersonTime;
        }
    }
    ```

4. Вызовите обработчик `CreatePersonCommandHandler`
    ```csharp
    [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            // Создание персоны и сохраниние в БД
            // ...
            return Task.FromResult(
                new PersonDto
                {
                    PersonId = Guid.NewGuid(),
                    Age = command.Age,
                    Gender = command.Gender,
                    FirstName = command.FirstName,
                    LastName = command.LastName
                });
        }
    }
    ```

5. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
    ```
    # HELP create_person_custom_time_metric Создание персоны уникальная  временная метрика
    # TYPE create_person_custom_time_metric gauge
    create_person_custom_time_metric 15.4418
    ```

### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
