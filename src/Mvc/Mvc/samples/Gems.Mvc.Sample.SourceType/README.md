# Источник данных FromQuery

Для того чтобы явно указать источник данных FromQuery необходимо сделать:

**Указать соответствующий SourceType у обработчика**

```csharp
[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons", SourceType = GenericControllers.SourceType.FromQuery)]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```

**В классе команды или запроса нужно указать атрибут FromQuery**. 
```csharp
public class CreatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    [FromQuery(Name = "firstName")]
    public string FirstName { get; set; }

    [FromQuery(Name = "lastName")]
    public string LastName { get; set; }

    [FromQuery(Name = "age")]
    public int Age { get; set; }
}
```

# Источник данных FromForm
Для того чтобы явно указать источник данных FromForm необходимо сделать:
**Указать соответствующий SourceType у обработчика**

```csharp
[Endpoint("api/v1/persons/import", "POST", OperationGroup = "Persons", SourceType = GenericControllers.SourceType.FromForm)]
public class ImportPersonCommandHandler : IRequestHandler<ImportPersonCommand>
{
    public Task Handle(ImportPersonCommand command, CancellationToken cancellationToken)
    {
        // Что-то делаем с command.CsvFile...
        throw new NotImplementedException();
    }
}
```

**В классе команды или запроса нужно указать атрибут FromForm**.
```csharp
public class ImportPersonCommand : IRequest
{
    [FromForm(Name = "csv_file")]
    public IFormFile CsvFile { get; set; }
}
```

# Источник данных FromBody
Для того чтобы явно указать источник данных FromBody необходимо сделать:
**Указать соответствующий SourceType у обработчика**

```csharp
[Endpoint("api/v1/persons", "PUT", OperationGroup = "Persons", SourceType = GenericControllers.SourceType.FromBody)]
public class UpdatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    public Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```

**В классе команды или запроса нужно указать атрибут FromBody**.
```csharp
public class UpdatePersonCommand : IRequest<Guid>, IRequestUnitOfWork
{
    [FromBody]
    public Person Person { get; set; }
}
```
