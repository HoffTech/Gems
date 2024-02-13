# Gems.Mvc

Эта библиотека .NET предоставляет общие классы для работы с MVC: ViewModels, Exceptions, фильтр для обработки исключений и ошибок валидации, регистрация контроллеров для обработчиков MediatR и тп. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка)
* [Использование исключений](#использование-исключений)
* [Переопределение текста и кода ошибки валидации входных данных](#переопределение-текста-и-кода-ошибки-валидации-входных-данных)
* [Регистрация контроллеров для обработчиков MediatR](#регистрация-контроллеров-для-обработчиков-mediatr)
* [Загрузка файлов](#загрузка-файлов)
* [Явное указание источника данных](#явное-указание-источника-данных)
* [Логирование](#логирование)
* [Метрики](#метрики)
* [Заголовки](#заголовки)

# Установка

- Установите nuget пакет **Gems.Mvc** через менеджер пакетов.
- Зарегистрируйте сервисы, добавив следующую строку:
```csharp
services.AddControllersWithMediatR(options => options.RegisterControllersFromAssemblyContaining<Startup>());
// Внимание: данный метод необходимо вызывать вместо стандартного: services.AddControllers()
```
- После строки **app.UseRouting();** добавьте следующие строки:
```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
```

# Использование исключений
Метод AddControllersWithMediatR регистрирует фильтр для обработки ошибок **HandleErrorFilter**, который перехватывает и обрабатывает исключения:
- Исключение **UnauthorizedAccessException** от библиотеки System после обработки возвращает StatusCodeResult со статусом 401.
- Исключение **ValidationException** от библиотеки FluentValidation после обработки возвращает BadRequestObjectResult со статусом 400.
- Исключение **InvalidDataException** от библиотеки System.IO после обработки возвращает BadRequestObjectResult со статусом 400.
- Исключение **InvalidOperationException** от библиотеки System.IO после обработки возвращает UnprocessableEntityObjectResult со статусом 422.
- Исключение **BusinessException** от библиотеки Gems.Mvc после обработки возвращает UnprocessableEntityObjectResult со статусом 422.
- Исключение **NotFoundException** от библиотеки Gems.Mvc после обработки возвращает NotFoundObjectResult со статусом 404.
- Исключение **ConflictException** от библиотеки Gems.Mvc после обработки возвращает ConflictObjectResult со статусом 409.
- Исключение **RequestException** в случае 502,503,504 от библиотеки Gems.Mvc после обработки возвращает ObjectResult со статусом 499.
- Исключение **SocketException** от библиотеки System.Net.Sockets после обработки возвращает ObjectResult со статусом 499.
- Исключение **PostgresException** в случае свойства SqlState, начинающихся с 08,28 или 3D от библиотеки Npgsql после обработки возвращает ObjectResult со статусом 499.
- Исключение **SqlException** в случае свойства, равного  53, 87 или 11001 от библиотеки Npgsql после обработки возвращает ObjectResult со статусом 499.
- Исключение **Exception** от библиотеки System после обработки возвращает ObjectResult со статусом 500.

Для всех исключений фильтр HandleErrorFilter формирует ошибку ввиде структуры  **BusinessErrorViewModel**:
```json
{
  "error": {
    "isBusiness": true,
    "message": "string",
    "code": "string",
    "errors": ["string"]
  }
}
```
# Переопределение текста и кода ошибки валидации входных данных
Реализуйте конвертер IValidationExceptionConverter, если нужно переопределить исключение ValidationException. Данное исключение выбрасывается при проверке валидаторами FluentValidation.       
Пример:
```csharp
public class ValidationExceptionToBusinessException : IValidationExceptionConverter<GetAvailablePayMethodsQuery>
{
    public Exception Convert(ValidationException exception, GetAvailablePayMethodsQuery query)
    {
        return new BusinessException("Некорректный запрос к сервису")
        {
            Error = { IsBusiness = true },
            StatusCode = 400
        };
    }
}
```
Зарегистрируйте конвертер, как сервис:
```csharp
services.AddConverter<ValidationExceptionToBusinessException>();
```
Реализуйте конвертер IModelStateValidationExceptionConverter, если нужно переопределить исключение ModelStateValidationException. Данное исключение выбрасывается при проверке методом ModelState.IsValid.
В генерик контроллерах это происходит перед вызовом метода контроллера. Данное исключение так же выброшено, если формат json-а на входе будет неправильный.           
Пример:
```csharp
public class ModelStateValidationExceptionToBusinessException : IModelStateValidationExceptionConverter<GetAvailablePayMethodsQuery>
{
    public Exception Convert(ModelStateValidationException exception, GetAvailablePayMethodsQuery query)
    {
        return new BusinessException("Некорректный запрос к сервису")
        {
            Error = { IsBusiness = true },
            StatusCode = 400
        };
    }
}
```
Зарегистрируйте конвертер, как сервис:
```csharp
services.AddConverter<ModelStateValidationExceptionToBusinessException>();
```

# Регистрация контроллеров для обработчиков MediatR
Метод AddControllersWithMediatR автоматически регистрирует контроллер для каждого обработчика, который промаркирован атрибутом **Endpoint**. Н-р:
```csharp
// запрос 
public class GetPersonQuery : IRequest<Dto.Person>
{
    [FromRoute]
    public Guid PersonId { get; set; }
}
// обработчик
[Endpoint("api/v1/persons/{PersonId}", "GET", OperationGroup = "Persons", Summary = "Получение информации о пользователе")]
public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, Dto.Person>
{
    public Task<Dto.Person> Handle(GetPersonQuery query, CancellationToken cancellationToken)
    {
        // Some code
    }
}
// где OperationGroup - группирует эндпоинты в сваггере. Summary - добавляет краткое описание эндпоинта в сваггере.

```
Поддерживаются такие типы обработчиков:
С возвращаемым параметром:
- IRequestHandler<SomeQuery,SomeDto>    => [Endpoint("api/v1", "GET")]
- IRequestHandler<SomeCommand, SomeDto> => [Endpoint("api/v1", "POST")]
- IRequestHandler<SomeCommand, SomeDto> => [Endpoint("api/v1", "PUT")]
- IRequestHandler<SomeCommand, SomeDto> => [Endpoint("api/v1", "PATCH")]

Без возвращаемого параметра:
- IRequestHandler<SomeCommand> => [Endpoint("api/v1", "DELETE")]
- IRequestHandler<SomeCommand> => [Endpoint("api/v1", "POST")]
- IRequestHandler<SomeCommand> => [Endpoint("api/v1", "PUT")]
- IRequestHandler<SomeCommand> => [Endpoint("api/v1", "PATCH")]

# Загрузка файлов
Для того чтобы загрузить файл необходимо указать чтение данных из формы, н-р:
```csharp
public class CreatePersonCommand : IRequest<Guid>, IRequestTransaction
{
	public string FirstName { get; set; }

	public string LastName { get; set; }

	public int Age { get; set; }

	public Gender Gender { get; set; }

	public IFormFile SomeFile { get; set; }
}

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons", IsForm = true)]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
	public async Task<Guid> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
	{
		using (var fileStream = new FileStream("c:\\temp\\SomeFile.txt", FileMode.Create))
		{
			await command.SomeFile.CopyToAsync(fileStream);
		}

		// other code
	}
}
```
# Явное указание источника данных
Для того чтобы явно указать источник данных (FromBody, FromQuery, FromForm) нужно использовать SourceType, н-р:
```csharp
public class CreatePersonCommand : IRequest<Guid>, IRequestTransaction
{
	public string FirstName { get; set; }

	public string LastName { get; set; }
}

[Endpoint("api/v1/persons", "POST", OperationGroup = "Persons", SourceType = SourceType.FromQuery)]
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Guid>
{
	public async Task<Guid> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
	{		
		// other code
	}
}
```
Данная возможность присутствует для Patch, Post и Put запросов. По умолчанию используется FromBody.
Также, аттрибут IsForm имеет более высокий приоритет.

# Логирование
Для обработчиков MediatR или классических контроллеров ASP.NET можно настроить автоматическую сбор логов для исходящих запросов. Как это сделать смотрите [здесь](/src/Logging/Mvc/README.md).

# Метрики
Многие метрики пишутся автоматически при подключении пайплайнов для Mediatr (см. [здесь](/src/Metrics/Metrics/README.md#использование-пайплайнов)).

# Заголовки
### Заголовок retry-after
Заголовок **retry-after** добавляется в Заголовки ответа сервиса при выбрасывании исключения _TooManyRequestException_ и возврата статуса 429.
<br/>

Для добавления заголовка **retry-after**:
1) Зарегистрируйте Pipeline (по умолчанию регистрируется в Gems.CompositionRoot)
```csharp
    this.services.AddPipeline(typeof(ReFireJobOnFailedBehavior<,>));
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
