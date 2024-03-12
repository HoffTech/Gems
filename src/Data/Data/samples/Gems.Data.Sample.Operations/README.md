# Типы операций UnitOfWork

### Основные понятия
- _CallTableFunctionFirstAsync_ - метод вызова функции для получения первого элемента выборки или значения по умолчанию
- _CallTableFunctionAsync_ - метод вызова функции для получения полного списка элементов выборки
- _CallScalarFunctionAsync_ - метод вызова скалярной функции для получения скалярного значения
- _CallStoredProcedureAsync_ - метод вызова процедуры, не возвращающей значения
- _QueryAsync_ - метод вызова SQL выражения для получения полного списка элементов выборки

> QueryAsync - удобно использовать при необходимости динамического формирования SQL выражения

### Параметры
1. `string functionName/storeProcedureName` - наименование функции/процедуры в БД
2. `commandTimeout` - таймаут на выполнение функции/процедуры в секундах
3. `DynamicParameters` или `Dictionary<string, object> parameters` - входящие параметры функции/процедуры
4. `Enum timeMetricType` - перечисление типа метрики (регистрируется по умолчанию в _Gems.CompositionRoot_ , но если требуется можно переопределить)

### Как работать с операциями
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

3) Получите объект **UnitOfWork** из провайдера **IUnitOfWorkProvider** и вызовите одну из доступных операций. В примере ниже вызываетяся процедура создания объекта **Person** с случайными характеристиками
```csharp
    public Task Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync("public.person_create");
    }
```

### Примеры вызова операций
1. _CallTableFunctionFirstAsync_
```csharp
    public async Task<PersonDto> Handle(GetPersonQuery query, CancellationToken cancellationToken)
    {
        var person = await this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallTableFunctionFirstAsync<Person>(
                "public.person_get_person_by_id",
                new Dictionary<string, object>
                {
                    ["p_person_id"] = query.PersonId
                });
    
        return this.mapper.Map<PersonDto>(person);
    }
```
2. _CallTableFunctionAsync_
```csharp
    public async Task<List<PersonDto>> Handle(GetPersonsQuery query, CancellationToken cancellationToken)
    {
        var persons = await this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallTableFunctionAsync<Person>(
                "public.person_get_persons",
                new Dictionary<string, object>
                {
                    ["p_skip"] = query.Skip ?? default,
                    ["p_take"] = query.Take ?? 100
                });

        return this.mapper.Map<List<PersonDto>>(persons);
    }
```
3. _CallScalarFunctionAsync_
```csharp
    public Task<int> Handle(GetPersonAgeQuery query, CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallScalarFunctionAsync<int>(
                "public.person_get_age_by_id",
                new Dictionary<string, object>
                {
                    ["p_person_id"] = query.PersonId
                });
    }
```
4. _CallStoredProcedureAsync_
```csharp
    public Task Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        return this.unitOfWorkProvider
            .GetUnitOfWork(cancellationToken)
            .CallStoredProcedureAsync(
                "public.person_create",
                new Dictionary<string, object>
                {
                    ["p_person"] = this.mapper.Map<Person>(command.Person)
                });
    }
```
5. _QueryAsync_
```csharp
        public async Task<List<PersonDto>> Handle(GetPersonByFilterQuery query, CancellationToken cancellationToken)
        {
            var person = await this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .QueryFirstOrDefaultAsync<Person>(CreateQuery(query, out var parameters), parameters);

            return this.mapper.Map<List<PersonDto>>(person);
        }
```

### Запуск примера
1. Для проверки сэмпла, настройте подключение~~~~ к реальной БД
2. Реализуйте функции/процедуры 
   1. Получение объекта **Person** по id `public.person_get_person_by_id`
   2. Получение списка объектов **Person** с учетом пагинации `public.person_get_persons`
   3. Получение возраста объекта **Person** по id `public.person_get_age_by_id`
   4. Создание объекта **Person**  `public.person_create`
4. Вызовите ендпоинты с помощью **Swagger**
    1. `api/v1/persons/update` 
    2. `api/v1/persons/update` 
    3. `api/v1/persons/update` 
    4. `api/v1/persons/update` 
    5. `api/v1/persons/update` 
