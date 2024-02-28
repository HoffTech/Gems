# Проброс исключений для доменных обработчиков

При обработке запроса обычно исключения логируются и пробрасываются далее, но иногда бывает так, что надо пробрасывать исключение только при выполнении определённых условий.
Например, если вы работаете с коллекций объектов и вам нужно не прерывать обработку, даже если произошла ошибка при обработке одного из них.

**У команды необходимо реализовать интерфейс IRequestException**
```csharp
public class ImportPersonCommand : IRequest, IRequestUnitOfWork, IRequestException
{
    public Person Person { get; set; }

    public bool NeedThrowException(Exception exception)
    {
        return exception is BusinessException { StatusCode: >= 499 };
    }
}
```

**Теперь проброс исключения при обработке объектов может произойти только если выполнено условие метода NeedThrowException**. 
```csharp
        foreach (var person in persons)
        {
            await this.mediator.Send(
                new ImportPersonCommand { Person = person },
                cancellationToken);
        }
```