# Административная конечная точка

Вы можете добавить в ваше приложение Asp.Net Core административную точку, которая позволит вам управлять настройками трассировки в ходе работы приложения. Для этого внутри вызова `app.UseEndpoints()` добавьте строку:

```
app.UseEndpoints(endpoints =>
{
    endpoints.MapTracingApi();
    . . .
});
```

По умолчанию создается конечная точка `/trace` которая поддерживает метод POST. Вы можете переопределить ее, задав настройки:

```
app.UseEndpoints(endpoints =>
{
    endpoints.MapTracingApi(
        new Gems.OpenTelemetry.Api.TracingApiOptions 
        { 
            Endpoint = "/_sys/tracing", 
        });
    . . .
});
```

## Использование административной конечной точки

Для того, чтобы изменить настройки трассировки, сделайте запрос:

```
curl --location 'http://localhost:8082/trace' \
--header 'Content-Type: application/json' \
--data '{
    "enabled": true
}'
```

Полный JSON для конечной точки:

```
{
    "enabled": true,
    "requestIn": {
        "include": [
            "/api/product/*"
        ],
        "exclude": [
            "/swagger*"
        ]        
    },
    "requestOut": {
        "include": [
            "https://my.site/*"
        ],
        "exclude": [
            "https://external/restricted/*"
        ]
    },
    "command": {
        "includeRequest": true,
        "includeResponse": true
    },
    "source": {
        "include": [ "*" ],
        "exclude": [ "*SqlClient*" ]
    },
    "mssql": {
        "command": {
            "include": [ "." ],
            "exclude": [ "\\sdbo\\.Users\\s" ]
        }
    }
}
```