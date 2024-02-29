# Gems.BusinessRules

# Применение бизнес-правил

**[Пример кода](/src/BusinessRules/BusinessRules/samples/Gems.BusinessRules.Sample.BusinessRulesUsing)**

Бизнес-правила представляют собой специализированный вид логики, описывающей ограничения. Необходимо различать бизнес-правила от валидации. Например, проверка возраста выдачи водительского удостоверения. Валидаторы проверяют, что число должно быть неотрицательное (больше 0). А бизнес правило, что число должно быть больше 18.  
Другой пример бизнес-правила: чтобы заказчик получил письмо, почтовый индекс на письме должен соответствовать индексу, в котором проживает заказчик.  

Вообще лучше для всех ограничений, которые описываются в техническом задании, представлять ввиде бизнес-правил.  
В логике валидации лучше оставлять обычные ограничения, по большей части - технические ограничения. Н-р: длина поля 20 символов, максимальное вводимое число, ввод неотрицательного числа, правильный формат даты и времени и тп. Может быть даже так, что лучше описать логику и в валидаторе и в бизнес-правиле, если ограничания совпадают.

Бизнес-правило должно реализовать один из следующих интерфейсов:
```csharp
public interface IBusinessRule<in TArg0, in TArg1, in TArg2, in TArg3, in TArg4> // аргументы могут отсутствовать
{
    bool IsBroken(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, out string errorMessage);
}

public interface IBusinessRuleAsync<in TArg0, in TArg1, in TArg2, in TArg3, in TArg4>
{
    Task<(bool IsBroken, string ErrorMessage)> BreakAsync(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken cancellationToken);
}

```
Реализация метода bool IsBroken(out string errorMessage) или Task<(bool IsBroken, string ErrorMessage)> BreakAsync()  должна содержать логику, проверяющая бизнес-правило и в случае нарушения, 
записывать сообщение в параметр errorMessage и возвращать true. Если нарушение бизнес-правила не было, то сообщение оставлять пустым и возвращать просто false.

Класс с реализацией бизнес-правила можно создавать, вызывая конструктор new. Н-р бизнес-правило, проверяющее данные заявления возврата и данные, полученные с аксапты (приложение Sales.ReturnClaim):
```csharp
var businessRule = new ClaimAndResponseMustBeEquivalentBusinessRule(returnClaim, claimResponse);
if (businessRule.IsBroken(out string errorMessage))
{
    throw new InvalidOperationException(errorMessage);
}
```
Если класс с реализацией бизнес-правила содержит зависимости, то необходимо создать фабрику, содержащая все эти зависимости. Н-р бизнес-правило, проверяющее тип возврата с разрешенными типами возврата в настройках, хранящихся в бд (приложение Sales.ReturnClaim):
```csharp
// бизнес-правило
public class ReturnTypeMustBeAllowedBusinessRule : IBusinessRule
{
    private readonly IConnectionStringProvider connectionStringProvider;
    private readonly ReturnType returnType;
    private readonly string paymentType;

    public ReturnTypeMustBeAllowedBusinessRule(IConnectionStringProvider connectionStringProvider, ReturnType returnType, string paymentType)
    {
        this.connectionStringProvider = connectionStringProvider;
        this.returnType = returnType;
        this.paymentType = paymentType;
    }

    public bool IsBroken(out string errorMessage)
    {    
        errorMessage = null;
        // ...
        return errorMessage != null;
    }
}

// фабрика
public class ReturnTypeMustBeAllowedBusinessRuleFactory
{
    private readonly IConnectionStringProvider connectionStringProvider;

    public ReturnTypeMustBeAllowedBusinessRuleFactory(IConnectionStringProvider connectionStringProvider)
    {
        this.connectionStringProvider = connectionStringProvider;
    }

    public IBusinessRule Create(ReturnType returnType, string paymentType)
    {
        return new ReturnTypeMustBeAllowedBusinessRule(this.connectionStringProvider, returnType, paymentType);
    }
}

// регистрация фабрики
services.AddSingleton<ReturnTypeMustBeAllowedBusinessRuleFactory>();

// использование
public class SomeClass
{
    private readonly ReturnTypeMustBeAllowedBusinessRuleFactory returnTypeBusinessRuleFactory;

    public SomeClass(ReturnTypeMustBeAllowedBusinessRuleFactory returnTypeBusinessRuleFactory)
    {
        this.returnTypeBusinessRuleFactory = returnTypeBusinessRuleFactory;
    }

    public void EnsureValidReturnType(Entities.ReturnClaim returnClaim)
    {
        foreach (var payment in returnClaim.Orders.SelectMany(x => x.Payments))
        {
            var businessRule = this.returnTypeBusinessRuleFactory.Create(ReturnType.Full, payment.PaymentType);
            if (!businessRule.IsBroken(out string errorMessage))
            {
                continue;
            }

            // другой код

            throw new InvalidOperationException(errorMessage);
        }
    }

    // другой код
}
```
Если бизнес-правил для одной фичи много, то можно использовать паттерн фасад и все бизнес правила включить в один класс, н-р BusinessRulesChecker:
```csharp
public class BusinessRulesChecker
{
    private readonly OrdersMustNotHaveDuplicatesBusinessRuleFactory duplicatesBusinessRuleFactory;
    private readonly ReturnTypeMustBeAllowedBusinessRuleFactory returnTypeBusinessRuleFactory;
    private readonly AmountMustBeLessThenAmountMaxBusinessRuleFactory amountBusinessRuleFactory;

    public BusinessRulesChecker(
        OrdersMustNotHaveDuplicatesBusinessRuleFactory duplicatesBusinessRuleFactory,
        ReturnTypeMustBeAllowedBusinessRuleFactory returnTypeBusinessRuleFactory,
        AmountMustBeLessThenAmountMaxBusinessRuleFactory amountBusinessRuleFactory)
    {
        this.duplicatesBusinessRuleFactory = duplicatesBusinessRuleFactory;
        this.returnTypeBusinessRuleFactory = returnTypeBusinessRuleFactory;
        this.amountBusinessRuleFactory = amountBusinessRuleFactory;
    }

    public void EnsureNotDuplicates(Entities.ReturnClaim returnClaim)
    {
        var businessRule = this.duplicatesBusinessRuleFactory.Create(returnClaim);
        if (businessRule.IsBroken(out string errorMessage))
        {
            throw new InvalidDataException(errorMessage);
        }
    }

    public void EnsureValidReturnType(Entities.ReturnClaim returnClaim)
    {
        foreach (var payment in returnClaim.Orders.SelectMany(x => x.Payments))
        {
            var businessRule = this.returnTypeBusinessRuleFactory.Create(ReturnType.Full, payment.PaymentType);
            if (!businessRule.IsBroken(out string errorMessage))
            {
                continue;
            }

            // другой код
            throw new InvalidOperationException(errorMessage);
        }
    }

    public void EnsureReturnConditions(Entities.ReturnClaim returnClaim)
    {
        var businessRule = this.amountBusinessRuleFactory.Create(returnClaim);
        if (businessRule.IsBroken(out string errorMessage))
        {
            // другой код
            throw new InvalidOperationException(errorMessage);
        }
    }
}
```

FYI: Бизнес-правила могут реализовывать интерфейсы с аргументами от TArg0 до TArg4. В таком случае, тогда можно обходится и без фабрик - внедрять зависимости через конструктор бизнес-правила, а контексты передавать через аргументы метода IsBroken. Но тогда нельзя будет объединять бизнес-правила в коллекцию(или использовать другие паттерны) и иметь единый интерфейс для вызова метода IsBroken. Чаще всего это и не нужно, поэтому использование бизнес-правил с аргументами является предпочтительней, так как для этого необходимо писать меньше кода.