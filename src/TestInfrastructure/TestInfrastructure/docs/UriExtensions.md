# Расширения для Uri

```csharp
public static Uri Add(this Uri uri, string relativeUri)
```

Добавляет к `Uri` относительный путь. При этом удаляется символ `/` в конце `uri` и в начале `relativeUri`. Пример использования: 

```csharp
using var app = new TestApplicationBuilder<Program>()
    .UseConnectionString("TemperatureInfo", this.env.WireMockServerUri("Default").Add("/TemperatureInfo/"))
    .UseConnectionString("PrecipitationInfo", this.env.WireMockServerUri("Default").Add("/PrecipitationInfo/"))
    .Build();
```

