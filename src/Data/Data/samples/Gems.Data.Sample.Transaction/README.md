# Работа с транзакциями

### Основные понятия
- Транзакция - последовательность операторов языка SQL, которая рассматривается как некоторое неделимое действие над базой данных, осмысленное с точки зрения пользователя
- **IRequestUnitOfWork** - интерфейс, позволяющий открывать транзакцию в рамках одного обработчика или бизнес-сценария целиком
- **UnitOfWorkBehavior** - класс поведения (этап пайплайна) в котором происходит коммит транзакции по завершению выполнения основых операций обработчика

> Если используется интерфейс _IRequestUnitOfWork_, то соединение не высвобождается до момента закрытия транзакции

### Как работать с транзакциями
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
```
2) Имплементируйте интерфейс _IRequestUnitOfWork_ в классе команды/запроса
```csharp
    public class UpdatePersonCommand : IRequest, IRequestUnitOfWork
    {
        public string UpdatedBy { get; init; }

        public PersonDto Person { get; init; }
    }
```
3) Последовательно вызовите несколько операций БД
```csharp
    public async Task Handle(UpdatePersonCommand command, CancellationToken cancellationToken)
    {
        await this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync(
                "public.person_update_person",
                new Dictionary<string, object>
                {
                    ["p_person"] = this.mapper.Map<Person>(command.Person)
                });

        await this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync(
                "public.log_update_log",
                new Dictionary<string, object>
                {
                    ["p_updated_by"] = command.UpdatedBy
                });
    }
```


### Запуск примера
1. Для проверки сэмпла, настройте подключение к реальной БД
2. Реализуйте процедуры 
   1. Обновление объекта **Person** `public.person_update_person`
   2. Обновление лога `public.log_update_log`
4. Вызовите ендпоинт `api/v1/persons/update` с помощью **Swagger**
