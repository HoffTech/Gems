# Возможности для 404 ответа

В Gems.Mvc есть возможность проверять ответ сервиса на null и автоматически возвращать 404 статус
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(NotFoundBehavior<,>));
```
2) Имплементируйте интерфейс IRequestNotFound для команды/запроса
```csharp
public class GetPersonQuery : IRequest<Person>, IRequestNotFound
{
    [FromRoute]
    public int PersonId { get; set; }

    public string GetNotFoundErrorMessage()
    {
        return $"Не найден пользователь с Id: {this.PersonId}";
    }
}
```