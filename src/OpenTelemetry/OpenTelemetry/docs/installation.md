# Установка и настройка Gems.OpenTelemetry

## Подключение пактов Nuget

Подключите к вашему приложению пакет Gems.OpenTelemetry командой 

```
dotnet add package Gems.OpenTelemetry 
```

Если ваше приложение использует Gems.Mvc, то также подключите пакет Gems.OpenTelemetry.Mvc

```
dotnet add package Gems.OpenTelemetry.Mvc
```

## Подключение к Grafana Tempo

Для включения поддержки трассировки, добавте в ваше приложение вызов:

```
services.AddDefaultTracing(configuration);
```

Данный метод настраивает вашу трассировку использу настройки приложения. Для настройки заполните секцию `Tracing`.

_appsettings.json_:

```
  "Tracing": {
    "Endpoint": "http://grafana:4317"
  }
```

Здесь Endpoint ссылается на grpc порт Grafana Tempo. 
По-умолчанию трассировка отключена. Если вы хотите ее включить, то нужно выставить параметр `Enabled` в значение `true`.

_appsettings.json_:

```
  "Tracing": {
    "Endpoint": "http://grafana:4317",
    "Enabled": true
  }
```
