# Gems.Http

Вспомогательная библиотека для выполнения Http запросов.

# Содержание
* [Установка](#установка)
* [Конфигурация](#конфигурация)
* [Описание](#описание)
* [Устаревшие методы](#устаревшие-методы)
* [Примеры](#примеры)
* [Логирование](#логирование)
* [Метрики](#метрики)
* [Аутентификация/Авторизация](#аутентификацияавторизация)

# Установка
Установите nuget пакет Gems.Http через менеджер пакетов

# Конфигурация

1. Добавьте следующую строку в конфигурацию сервисов:
```csharp
services.AddHttpServices(this.Configuration);
```
2. Добавьте конфигурацию настроек выполнения http запросов в файле appsettings.json:
```csharp
"HttpClientServiceOptions": {
    "Durable": false,                       // true - запрос будет повторяться указанное количество раз в случае ошибки.
    "DurableFromHttpStatus": 499,           // Начиная с какого http статуса делать повторные запросы в случае ошибки. По умолчанию 499.
    "Attempts": 0,                          // Количество повторных попыток в случае если отправка запроса не удалась. По умолчанию 3.
    "MillisecondsDelay": 0,                 // Количество милисекунд задержки перед повторной отправкой. По умолчанию 5000.
    "RequestTimeout": 30000,                // Время ожидания в милисекундах для выполнения запроса. По умолчанию 30 секунд
    "NeedDownloadCertificate": false,       // Необходимо загрузить сертификат.
    "BaseUrl": "http://localhost:5135/"     // Базовый url сервиса.
}
```

# Описание
Для выполнения http запросов доступен класс DefaultClientService.

Данный класс наследуется от класса BaseClientService&lt;BusinessErrorViewModel&gt;. Где BusinessErrorViewModel - эта стандарт модели ошибки, принятый в Hoff Tech. 
```json
{

    "error": {
        "isBusiness": boolean,    // Признак, характеризующий тип ошибки: true - если ошибка предусмотрена бизнес логикой, false - если возникло незапланированное исключение (exception).
        "message": "string",    // Текст ошибки. В случае isBusiness = true текст должен быть представлен в человеко-читаемом виде, на понятном для пользователя языке. В случае isBusiness = false - достаточно вывести текст исключения.
        "code": "string",   //NULL    // Код ошибки/исключения
        "errors": [     //NULL
            "error description1",
            "error description2"
        ]
    }
}
```

Класс DefaultClientService не имеет методов и не переопределяет свойства и методы, наследуемого класса. Поэтому, если требуется настроить свой клиент, то необходимо создать свой класс и затем не забыть зарегистрировать в сервисах:
```csharp
services.AddSingleton<YourClientService>();
```
Абстрактный класс BaseClientService&lt;BusinessErrorViewModel&gt; предоставляет защищенные виртуальные свойства и методы, которые позволяют настроить клиент под свои нужды:
```csharp
/// <summary>
/// MediaType по умолчанию.
/// </summary>
protected virtual string MediaType => "application/json";

/// <summary>
/// Базовый url сервиса.
/// </summary>
protected virtual string BaseUrl => string.Empty;

/// <summary>
/// true - запрос будет повторяться указанное количество раз в случае ошибки.
/// Переопределяет значение из настроек.
/// </summary>
protected virtual bool Durable => this.options?.Value?.Durable ?? false;

/// <summary>
/// Количество повторных попыток в случае если отправка запроса не удалась.
/// Переопределяет значение из настроек.
/// </summary>
protected virtual short Attempts => this.options?.Value?.Attempts ?? 3;

/// <summary>
/// Количество милисекунд задержки перед повторной отправкой.
/// Переопределяет значение из настроек.
/// </summary>
protected virtual int MillisecondsDelay => this.options?.Value?.MillisecondsDelay ?? 5000;

/// <summary>
/// Время ожидания в милисекундах для выполнения запроса.
/// Переопределяет значение из настроек.
/// </summary>
protected virtual int RequestTimeout => this.options?.Value?.RequestTimeout ?? 0;

/// <summary>
/// Конвертеры для десериализации в json.
/// </summary>
protected virtual IList<JsonConverter> DeserializeAdditionalConverters => null;

/// <summary>
/// Конвертеры для сериализации из json.
/// </summary>
protected virtual IList<JsonConverter> SerializeAdditionalConverters => null;

/// <summary>
/// true - сериализовать в Camel Case.
/// </summary>
protected virtual bool IsCamelCase => true;

/// <summary>
/// Получает токен доступа, который отправляется с Bearer.
/// </summary>
protected virtual Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
{
    return Task.FromResult<string>(null);
}
```
Абстрактный класс BaseClientService&lt;BusinessErrorViewModel&gt; предоставляет публичный виртуальный метод SendRequestAsync, который является основным методом выполняющий запрос: 

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.RequestMethods)**

```csharp
public virtual async Task<TResponse> SendRequestAsync<TResponse, TError>(
            HttpMethod httpMethod,
            TemplateUri templateUri,
            object requestData,
            IDictionary<string, string> headers,
            bool isAuthenticationRequest,
            CancellationToken cancellationToken)
        where TError : class, new();
```
Абстрактный класс BaseClientService&lt;BusinessErrorViewModel&gt; предоставляет публичный (уже не виртуальный)  метод TrySendRequestAsync, который вызывает метод внутри себя метод SendRequestAsync, 
оборачивая его в конструкцию try-catch. Исключение в этом случае не бросаестся а записывается в публичное свойство LastRequestException: 
```csharp
/// <summary>
/// Получает последнее исключение, которое возникло при вызове метода TrySendRequestAsync.
/// </summary>
public Exception LastException { get; private set; }
```
Остальные методы являются прокси методами и не виртуальные. То есть их нельзя переопределить. Данные методы имеют меньшее количество входных параметров. Основной метод сделан виртуальным для того, чтобы можно было мокать в тестах. 

Есть такие методы: 

- Методы, которые не имеют префикса Try - бросают исключение RequestException&lt;TDefaultError&gt;. Где TDefaultError это выше описанная модель ошибки, которая является генериком класса.

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.SendRequest)**

```csharp
Task<TResponse> PostAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> PutAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> PatchAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> DeleteAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> GetAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);

// есть так же перегрузки, которые не имеют параметр headers.
```

- Методы, которые имеют префикс Try - не бросают исключение RequestException&lt;TDefaultError&gt;, а возвращают ошибку вместе с результатом.

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.TrySendRequestt)**

```csharp
Task<(TResponse, TDefaultError)> TryPostAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<(TResponse, TDefaultError)> TryPutAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<(TResponse, TDefaultError)> TryPatchAsync<TResponse>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<(TResponse, TDefaultError)> TryDeleteAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<(TResponse, TDefaultError)> TryGetAsync<TResponse>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);

// есть так же перегрузки, которые не имеют параметр headers.
```

- Методы, которые не имеют генерик тип - не читают тело ответа, а возвращают значение Unit (пустая структура).
```csharp
Task<Unit> PostAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<Unit> PutAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<Unit> PatchAsync(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<Unit> DeleteAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<Unit> GetAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);

// есть так же перегрузки, которые не имеют параметр headers и перегрузки с префиксом Try.
```

- Методы, которые имеют постфикс WithCustomErrorAsync - позволяют переопределить модель ошибки TDefaultError своей кастомной моделью.

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.SendRequestWithCustomError)**

```csharp
Task<TResponse> PostWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> PutWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> PatchWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, object requestData, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> DeletetWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);
Task<TResponse> GettWithCustomErrorAsync<TResponse, TError>(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken);

// есть так же перегрузки, которые не имеют параметр headers, перегрузки с префиксом Try и перегрузки, возвращающие Unit.
```

Есть перегрузки методов для get запросов, которые не имеют генерик типа для ответа:

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.RequestMethods)**

```csharp
Task<string> GetStringAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
Task<Stream> GetStreamAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)
Task<byte[]> GetByteArrayAsync(TemplateUri templateUri, IDictionary<string, string> headers, CancellationToken cancellationToken)

// есть так же перегрузки, которые не имеют параметр headers, перегрузки с префиксом Try, перегрузки с постфиксом WithCustomErrorAsync и перегрузки, возвращающие Unit.
```

# Логирование
В BaseClientService можно настроить автоматическую сбор логов для исходящих запросов. Как это сделать смотрите [здесь](/src/Logging/Mvc/README.md#сбор-логов-requestlogscollector).

1. **Введения дополнительного логирования параметров тела запроса в _BaseClientService_**

**[ Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Logging.AddLogsFromPayload)**

2. **Сокрытие чувствительных данных при логировании Http запросов**

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Logging.Security)**

# Метрики
В BaseClientService можно настроить автоматическую запись метрик для исходящих запросов. Как работать с метриками смотрите [здесь](/src/Metrics/Http/README.md#метрики-с-baseclientservice).

1. **Глобальное переопределение наименования метрик в _BaseClientService_**

**[ Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Metrics.GlobalOverride)**

2. **Частное переопределение наименования метрики в _BaseClientService_ для отдельного запроса**

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Mertics.RequestOverride)**


# Аутентификация/Авторизация

1. **Basic авторизация**

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Authentication.Basic)**

2.  **Jwt авторизация**

**[Пример кода](/src\Http\Http\samples\Gems.Http.Samples.Authentication.Jwt)**

Для возможности аутентификации/авторизации предусмотрен виртуальный метод:
```csharp
protected virtual Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
{
    return Task.FromResult<string>(null);
}
```

Внутри метода предполагается инкапуслирование функции Логина к стороннему Api, позволяющее получить AccessToken

Например: 
```csharp
protected override Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
{
    return this
        .SendAuthenticationRequestAsync<string>(
            requestUri: "api/users/login".ToTemplateUri(),
            requestData: new LoginRequestDto
            {
                UserName = options.Value.UserName,
                Password = options.Value.Password
            },
            headers: null,
            cancellationToken);
}
```

Логика работы аутентификации посредством передачи токена:
1) При выполнении каждого запроса вызывается _GetAccessTokenAsync_ для передачи в _Headers_ запроса, если AccessToken равен пустой строке или null.
2) В случае если на запрос из п.1 вернулась ошибка **401** или **403**(т.е. токен утратил актуальность), AccessToken устанавливается в значение null - происходит повторная попытка п.1.
3) Если на запрос возвращается ошибка 401 в течении 3 раз, то ошибка выбрасывается наверх.

Вышеприведенная логика подразумевает обновление токена только в случае утраты актуальности.