# Использование пайплайна TimeMetricBehavior по умолчанию

**Для того чтобы использовать пайплайн _TimeMetricBehavior_ необходимо:**
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

2. Вызовите обработчик `CreatePersonCommandHandler`
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

3. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
    ```
    # HELP create_person_time create person time
    # TYPE create_person_time gauge
    create_person_time 1.9137
    ```


### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
