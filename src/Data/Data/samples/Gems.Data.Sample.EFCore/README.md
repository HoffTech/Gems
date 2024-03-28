# Работа с EF Core

### Основные понятия
- _EntityFramework Core_ - объектно-ориентированная технология для доступа к данным и ORM-инструмент для  отображения данных на реальные объекты
- _DbContext_ - это сочетание шаблонов единиц работы и репозитория (область действия _UnitOfWork_)
- _IDbContextProvider_ - провайдер для получения контекста данных (_DbContext_)
- _UnitOfWorkBehavior_ - класс поведения (этап пайплайна), в котором происходит создание контекста (если он ранее не был создан) и коммит транзакции. По окончанию работы вызывается метод _SaveChanges_ и закрытие транзакции

> Работа с _IDbContextProvider_ и _DbContext_ аналогична работе с стандартным _IUnitOfWorkProvider_ и _UnitOfWork_ (транзакции, вложенные обработчики) **за исключением метрик**

### Как работать с операциями
1) Регистрация _UnitOfWork_ осуществляется в классе _Startup_
```csharp
    opt.AddUnitOfWorks = () =>
    {
        services.AddDbContextFactory<ApplicationDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!));
        
        services.AddDbContextFactory<MigrationDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("MigrationConnection")!));

        // если ранее были зарегистрированы стандартные UnitOfWork, то вызывать необязательно
        services.AddDbContextProvider();
    };
```

2) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(UnitOfWorkBehavior<,>));
```
3) Инъектируйте в конструктор класса интерфейс **IDbContextProvider**
```csharp
        private readonly IDbContextProvider dbContextProvider;

        public GetPersonByIdQueryHandler(IDbContextProvider dbContextProvider)
        {
            this.dbContextProvider = dbContextProvider;
        }
```
4) Получите объект **DbContext** из провайдера **IDbContextProvider**, таким образом откроется стандартное Api для работы с **DbContext**. В примере ниже вызываетяся процедура получения объекта **Person** по _id_
```csharp
public async Task<PersonDto> Handle(GetPersonByIdQuery query, CancellationToken cancellationToken)
{
    var dbContext = await this.dbContextProvider
        .GetDbContext<ApplicationDbContext>(cancellationToken)
        .ConfigureAwait(false);

    return this.mapper.Map<PersonDto>(
        await dbContext.Set<Person>().FirstOrDefaultAsync(p => p.PersonId == query.Id, cancellationToken));
}
```

### Запуск примера
1. Для проверки сэмпла, настройте подключение к реальной БД
2. Подготовьте инфраструктуру БД
4. Вызовите ендпоинт `GET /api/v1/persons/{id}` с помощью **Swagger**

