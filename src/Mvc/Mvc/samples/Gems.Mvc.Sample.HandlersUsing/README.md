# Простейший обработчик HTTP запроса

Этот пример демонстрирует как зарегистрировать обработчик HTTP запроса, а также основные базовые операции, такие как валидация входных данных и тд

# Содержание
* [Регистрация обработчика HTTP запроса](#регистрация-обработчика-http-запроса)
* [Валидация входных данных](#валидация-входных-данных)
* [Переопределение текста и кода ошибки валидации входных данных](#переопределение-текста-и-кода-ошибки-валидации-входных-данных)

# Регистрация обработчика HTTP запроса

**Зарегистрируйте контроллер для каждого обработчика, который промаркирован атрибутом Endpoint**. 

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
**Опишите класс запроса и его обработчик**
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
# Валидация входных данных
Для валидации входных данных используется функционал FluentValidation.
Для добавления валидации:
1. Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(ValidatorBehavior<,>));
```
2. Реализуйте валидатор команды или запроса:
```csharp
public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        this.RuleFor(m => m.FirstName)
            .NotEmpty()
            .MaximumLength(20);

        this.RuleFor(m => m.LastName)
            .NotEmpty()
            .MaximumLength(20);

        this.RuleFor(m => m.Age)
            .GreaterThanOrEqualTo(0);
    }
}
```

# Переопределение текста и кода ошибки валидации входных данных

**Реализуйте конвертер IValidationExceptionConverter, если нужно переопределить исключение ValidationException.**
Данное исключение выбрасывается при проверке валидаторами FluentValidation, а также при проверке методом ModelState.IsValid.
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
**Зарегистрируйте конвертер, как сервис**
```csharp
services.AddConverter<ValidationExceptionToBusinessException>();
```