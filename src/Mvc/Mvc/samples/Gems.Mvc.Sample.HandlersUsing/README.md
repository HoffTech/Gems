# Простейший обработчик HTTP запроса, а также пример переопределения текста и кода ошибки валидации входных данных

Этот пример демонстрирует как зарегистрировать обработчик HTTP запроса, а также переопределить текст и код ошибки валидации входных данных

# Регистрация обработчика HTTP запроса

##Зарегистрируйте контроллер для каждого обработчика, который промаркирован атрибутом **Endpoint**. 

Это можно сделать двумя способами:
1. В файле Startup.cs добавьте следующий код
```csharp
public void ConfigureServices(IServiceCollection services)
{ 
	services.AddControllersWithMediatR(options => options.RegisterControllersFromAssemblyContaining<Startup>());		
}
```
2. С помощью паттерна CompositionRoot
```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(
            configuration,
            opt =>
            {
                opt.AddUnitOfWorks = () => { };
                opt.AddPipelines = () =>
                {
                    services.AddHttpContextAccessor();
                    services.AddPipeline(typeof(ScopeLoggingBehavior<,>));
                    services.AddPipeline(typeof(EndpointLoggingBehavior<,>));
                    services.AddPipeline(typeof(NotFoundBehavior<,>));
                    services.AddPipeline(typeof(ExceptionBehavior<,>));
                    services.AddPipeline(typeof(ErrorMetricsBehavior<,>));
                    services.AddPipeline(typeof(TimeMetricBehavior<,>));
                    services.AddPipeline(typeof(ValidatorBehavior<,>));
                };
            });
    }
```
##Опишите класс запроса и его обработчик
```csharp
// запрос
public class CreatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }
}

// обработчик
[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

```

# Переопределение текста и кода ошибки валидации входных данных

##Реализуйте конвертер IValidationExceptionConverter, если нужно переопределить исключение ValidationException. Данное исключение выбрасывается при проверке валидаторами FluentValidation. 
Данное исключение выбрасывается при проверке методом ModelState.IsValid.
В генерик контроллерах это происходит перед вызовом метода контроллера. Данное исключение так же выброшено, если формат json-а на входе будет неправильный. 

```csharp
public class ValidationExceptionToBusinessException : IValidationExceptionConverter<CreatePersonCommand>
{
    public Exception Convert(ValidationException exception, CreatePersonCommand command)
    {
        return new BusinessException("Некорректный запрос к сервису")
        {
            Error = { IsBusiness = true },
            StatusCode = 400
        };
    }
}

```
##Зарегистрируйте конвертер, как сервис
```csharp
services.AddConverter<ValidationExceptionToBusinessException>();
```