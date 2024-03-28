# Контекст

### Основные понятия
- Контекст - это область действия объекта _UnitOfWork_
- Контекст реализован на базе **Gems.Context**. Хранение объектов _UnitOfWork_ происходит в поле _AsyncLocal<IContext>_ класса _ContextAccessor_
- Структурная схема работы контекста по _StackTrace_ на базе _AsyncLocal_
```csharp
MethodAsync1()
{
    CurrentContext.Value = “data1”
    MethodAsync2()
    {
        “data1”
        MethodAsync3()
        {
            “data1”
            CurrentContext.Value = “data2”
            MethodAsync4()
            {
                “data2”
            }
            “data2”
            MethodAsync5()
            {
                “data2”
            }        
             “data2”           
        }
        “data1”
        MethodAsync6()
        {
             “data1”
         }
         “data1”
}
```
> Использование контекста позволяет создавать вложенные транзакции без необходимости создания нового _CancellationToken_ (_LinkedTokenSource_) для вложенного UnitOfWork.
> > Каждый уникальный объект _UnitOfWork_ предполагает свою уникальную транзакцию

### Как работать с контекстом
1) Добавьте использование локального контекста для `UnitOfWorkProvider` в `appsettings.json`

```json
"UnitOfWorkProviderOptions": {
    "UseContext": true
  }
```

2) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
```

3) Создайте Команду и Обработчик для вложенной транзакции
```csharp
    public class UpdatePersonInternalCommand : IRequest, IRequestUnitOfWork
    {
        public string UpdatedBy { get; set; }

        public Person Person { get; set; }
    }
```

4) Вызовите последовательно верхнеуровневую логику и внутреннюю транзакцию
> Верхнеуровневая логика отрабатывает постоянно
> <br/>
> При этом внутренняя транзакция может быть отменена (rollback) 
```csharp
    public async Task Handle(UpdatePersonsCommand command, CancellationToken cancellationToken)
    {
        // 1. Сначала производим проверку на текущую сессию (создаем новую, обновляем количество запросов или сбрасываем)
        // Эту операцию необходимо выполнить прежде всего отдельно от транзакции, чтобы вести счетик запросов
        await this.ProcessSessionAsync(command, cancellationToken);

        // 2. Затем в отдельной транзакции обновляем объект Person и объект Log
        await this.mediator
            .Send(
                new UpdatePersonInternalCommand
                {
                    Person = this.mapper.Map<Person>(command.Person),
                    UpdatedBy = command.UpdatedBy
                },
                cancellationToken)
            .ConfigureAwait(false);
    }
```

### Запуск примера
1. Для проверки сэмпла, настройте подключение к реальной БД
2. Реализуйте функции/процедуры
   1. Получение объекта **Session** по id `public.session_get_session_by_id`
   1. Создание объекта **Session** `public.session_create`
   2. Обновление поля `qty` для объекта **Session** по id`public.session_update_qty`
   3. Сброс объекта  **Session** по id `public.session_reset`
   4. Обновление объекта **Person** `public.person_update_person`
   5. Обновление лога `public.log_update_log`
3. Вызовите ендпоинт `api/v1/persons/update` с помощью **Swagger**
