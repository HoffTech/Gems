# Работа с БД через интерфейс IUnitOfWork

### Основные понятия
- **UnitOfWork** - это единица работы, позволяющая работать с БД в рамках единого контекста и соблюдать атомарность транзакций.

- **UnitOfWorkProvider** - класс провайдер, позволяющий получить доступ к объекту UnitOfWork

- **UnitOfWorkBehavior** - класс поведения (этап пайплайна), в котором происходит создание контекста (если он ранее не был создан) и коммит транзакции. Для каждой транзакции создается свой контекст

> Если выполняется атомарная операция в БД (н-р вызов процедуры) без явного использования транзакции с помощью интерфейса **IRequestUnitOfWork**, то Подключение к БД(Connection) высвобождается сразу после выполнения операции


### Как работать с **UnitOfWork**
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
```
2) Инъектируйте в конструктор класса интерфейс **IUnitOfWorkProvider**
```csharp
    private readonly IUnitOfWorkProvider unitOfWorkProvider;
    
    public CreatePersonCommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }
```

3) Получите объект **UnitOfWork** из провайдера **IUnitOfWorkProvider** и вызовите одну из доступных операций. В примере ниже вызываетяся процедура создания объекта **Person** с случайными х-ками
```csharp
    public Task Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync("public.person_create_random");
    }
```


### Запуск примера

1. Для проверки сэмпла, настройте подключение к реальной БД
2. Реализуйте процедуру создания объекта **Person** `public.person_create_random`
3. Вызовите ендпоинт `api/v1/persons/random` с помощью **Swagger**
