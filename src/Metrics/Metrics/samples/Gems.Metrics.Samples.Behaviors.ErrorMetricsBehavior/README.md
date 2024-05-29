# Использование пайплайна ErrorsMetricsBehavior

**Для того чтобы использовать пайплайн _ErrorsMetricsBehavior_ необходимо:**
1. В классе `Startup.cs` подключите маппинг метрик и зарегистрируйте пайплайн _ErrorMetricsBehavior_ (регистрируется по умолчанию в Gems.CompositionRoot)
    ```csharp
   public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(
            configuration,
            opt =>
            {
                opt.AddPipelines = () =>
                {
                    services.AddPipeline(typeof(Gems.Metrics.Behaviors.ErrorMetricsBehavior<,>));
   
                    // Дополнительно подключаем в пайплайн валидацию, для отлова ошибок валидации
                    services.AddPipeline(typeof(ValidatorBehavior<,>));
                };
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

3. Рассмотрим 3 сценария записи метрик _(ошибка валидации_, _ошибка по бизнес правилу_, _успешный сценарий_)
   1. **Ошибка валидации** 
      - Вызовите обработчик `CreatePersonCommandHandler`при этом не заполните параметры `FirstName` или `LastName` или `Age`
         ```csharp
         [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
          public class CreatePersonCommandHandler(BusinessRuleChecker businessRuleChecker) : IRequestHandler<CreatePersonCommand, PersonDto>
         ```
      - Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
          ```
          # HELP feature_counters 
          # TYPE feature_counters gauge
          feature_counters{feature_name="create_person",error_type="validation",status_code="400",custom_code="none"} 1

          # HELP errors_counter 
          # TYPE errors_counter gauge
          errors_counter{feature_name="create_person",error_type="validation",status_code="400",custom_code="none"} 1
          ```
   2. **Ошибка по бизнес правилу**
      - Вызовите обработчик `CreatePersonCommandHandler`при этом заполните параметры `FirstName`,`LastName` и `Age`, в параметр `Age` передайте значение менее **18**
         ```csharp
         [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
          public class CreatePersonCommandHandler(BusinessRuleChecker businessRuleChecker) : IRequestHandler<CreatePersonCommand, PersonDto>
         ```
      - Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
          ```
          # HELP feature_counters 
          # TYPE feature_counters gauge
          feature_counters{feature_name="create_person",error_type="business",status_code="422",custom_code="none"} 1
        
          # HELP errors_counter 
          # TYPE errors_counter gauge
          errors_counter{feature_name="create_person",error_type="business",status_code="422",custom_code="none"} 1
          ```
   3. **Успешный сценарий**
       - Вызовите обработчик `CreatePersonCommandHandler`при этом заполните параметры `FirstName`,`LastName` и `Age`, в параметр `Age` передайте значение более **18**
         ```csharp
         [Endpoint("api/v1/persons/create", "POST", OperationGroup = "Persons", Summary = "Создание персоны")]
          public class CreatePersonCommandHandler(BusinessRuleChecker businessRuleChecker) : IRequestHandler<CreatePersonCommand, PersonDto>
         ```
       - Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик
           ```
           # HELP feature_counters 
           # TYPE feature_counters gauge
           feature_counters{feature_name="create_person",error_type="none",status_code="200",custom_code="none"} 2
           ```

### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
