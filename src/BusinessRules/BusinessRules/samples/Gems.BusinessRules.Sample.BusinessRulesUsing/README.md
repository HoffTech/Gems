# Применение бизнес-правил

Этот пример демонстрирует как работать с бизнес-правилами.

Для применения бизнес-правила необходимо сделать следующее:

**Реализуйте интерфейс Gems.BusinessRules.IBusinessRule**. 

```csharp
public class PersonAgeBusinessRule : IBusinessRule<int>
{
    public bool IsBroken(int age, out string errorMessage)
    {
        if (age < 18)
        {
            errorMessage = "Человек не достиг 18 лет.";

            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
```

**Если бизнес-правил для одной фичи много, то можно использовать паттерн фасад и все бизнес правила включить в один класс, например BusinessRulesChecker:**
```csharp
public class BusinessRulesChecker
{
    private readonly PersonAgeBusinessRule personAgeBusinessRule;

    public BusinessRulesChecker(PersonAgeBusinessRule personAgeBusinessRule)
    {
        this.personAgeBusinessRule = personAgeBusinessRule;
    }

    public void CheckPersonAge(int age)
    {
        if (this.personAgeBusinessRule.IsBroken(age, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }
    }
}
```

**Зарегистрируйте в DI конфигурации конкретной фичи оба класса**
```csharp
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PersonAgeBusinessRule>();
        services.AddSingleton<BusinessRulesChecker>();
    }
```

**Бизнес-правило готово к применению**
```csharp
[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons")]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly BusinessRulesChecker businessRulesChecker;

    public CreatePersonCommandHandler(BusinessRulesChecker businessRulesChecker)
    {
        this.businessRulesChecker = businessRulesChecker;
    }

    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        this.businessRulesChecker.CheckPersonAge(request.Age);

        return Task.FromResult(Guid.Empty);
    }
}
```
