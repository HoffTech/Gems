# Добавление заголовка retry-after в тело ответа сервиса

Заголовок retry-after добавляет в тело ответа сервиса при выбрасывании исключения _TooManyRequestException_ и возврата статуса 429.
Для добавления заголовка **retry-after**:
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(AddRetryAfterHeaderBehavior<,>));
```
2) Имплементируйте интерфейс IRequestAddRetryAfterHeader для команды/запроса
```csharp
    public class SomethingCommand : IRequestAddRetryAfterHeader
    {
        public int GetRetryAfterInterval()
        {
            return 60; // по умолчанию 60
        }
    }
```