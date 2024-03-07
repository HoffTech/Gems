# Простейший обработчик HTTP запроса который показывает как загрузить файл

Для того, чтобы загрузить файл необходимо сделать следующее:

**У команды указать поле типа Microsoft.AspNetCore.Http.IFormFile в которым будет содержаться данные загружаемого файла**
```csharp
public class ImportPersonCommand : IRequest, IRequestUnitOfWork
{
    public IFormFile CsvFile { get; set; }
}
```

**У обработчика указать чтение данных из формы IsForm = true**. 
```csharp
[Endpoint("api/v1/persons/import", "POST", OperationGroup = "Persons", IsForm = true)]
public class ImportPersonCommandHandler : IRequestHandler<ImportPersonCommand>
{
    public Task Handle(ImportPersonCommand command, CancellationToken cancellationToken)
    {
        // Что-то делаем с command.CsvFile...
        throw new NotImplementedException();
    }
}
```