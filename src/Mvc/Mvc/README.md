# Gems.Mvc

Эта библиотека .NET предоставляет общие классы для работы с MVC: ViewModels, Exceptions, фильтр для обработки исключений и ошибок валидации, регистрация контроллеров для обработчиков MediatR и тп. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка)
* [Регистрация контроллеров для обработчиков MediatR](#регистрация-контроллеров-для-обработчиков-mediatr)
* [Валидация входных данных](#валидация-входных-данных)
* [Переопределение текста и кода ошибки валидации входных данных](#переопределение-текста-и-кода-ошибки-валидации-входных-данных)
* [Использование исключений](#использование-исключений)
* [Загрузка файлов](#загрузка-файлов)
* [Явное указание источника данных](#явное-указание-источника-данных)
* [Логирование](#логирование)
* [Метрики](#метрики)
* [Заголовки](#заголовки)
* [Возможности для 404 ответа](#возможности-для-404-ответа)

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

# Регистрация контроллеров для обработчиков MediatR
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.Sample.HandlersUsing)**

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

# Валидация входных данных
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.Sample.HandlersUsing)**

# Переопределение текста и кода ошибки валидации входных данных
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.Sample.HandlersUsing)**

# Использование исключений
Метод AddControllersWithMediatR регистрирует фильтр для обработки ошибок **HandleErrorFilter**, который перехватывает и обрабатывает исключения:
- Исключение **UnauthorizedAccessException** от библиотеки System после обработки возвращает StatusCodeResult со статусом 401.
- Исключение **ValidationException** от библиотеки FluentValidation после обработки возвращает BadRequestObjectResult со статусом 400.
- Исключение **InvalidDataException** от библиотеки System.IO после обработки возвращает BadRequestObjectResult со статусом 400. [Obsolete]
- Исключение **InvalidOperationException** от библиотеки System после обработки возвращает UnprocessableEntityObjectResult со статусом 422.
- Исключение **InvalidDataException** от библиотеки Gems.Mvc после обработки возвращает BadRequestObjectResult со статусом 400.
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

# Загрузка файлов
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.Sample.LoadFile)**

# Явное указание источника данных
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.Sample.SourceType)**

Для того чтобы явно указать источник данных (FromBody, FromQuery, FromForm) нужно использовать SourceType, например:
Данная возможность присутствует для Patch, Post и Put запросов. По умолчанию используется FromBody.
Также, аттрибут IsForm имеет более высокий приоритет.

# Логирование
Для обработчиков MediatR или классических контроллеров ASP.NET можно настроить автоматическую сбор логов для исходящих запросов. Как это сделать смотрите [здесь](/src/Logging/Mvc/README.md).

# Метрики
Многие метрики пишутся автоматически при подключении пайплайнов для Mediatr (см. [здесь](/src/Metrics/Metrics/README.md#использование-пайплайнов)).

# Заголовки
### Заголовок retry-after
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.AddRetryAfterHeader)**

# Возможности для 404 ответа
**[Пример кода](/src/Mvc/Mvc/samples/Gems.Mvc.NotFound)**

В Gems.Mvc есть возможность проверять ответ сервиса на null и автоматически возвращать 404 статус
