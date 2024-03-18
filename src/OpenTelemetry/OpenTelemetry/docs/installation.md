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

## Подключение обновления через feature flag

### Для включения обновления трассировки через feature flag в первую очередь надо включить поддержку [feature flags](../../../FeatureToggle/FeatureToggle) и [gitlab settings](../../../Settings/Gitlab):

Сначала в gitlab проекте необходимо сделать 2 вещи: 
- завести feature flag "tracing". Для этого на странице проекта в левом меню открываете Deployments -> Feature Flags. 
Далее, жмёте New feature flag, в поле Name обязательно пишете `tracing`, выбираете нужные среды, далее create.
- завести ci/cd переменную  "tracing". Для этого на странице проекта в левом меню открываете Settings -> CI/CD -> Variables.
Далее, жмёте Add variable, Key обязательно `TRACING`, Value см. [здесь](trace-endpoint.md), выбираете Scope, далее Add variable.  

Логика работы следующая - background service проверяет значение флага `tracing` раз в `FeatureFlagUpdateIntervalSeconds` секунд.
Если состояние флага поменялось с false на true, сервис загрузит значение переменной `tracing`, попытается десериализовать и применить к настройкам трассировки.
Если обновление прошло успешно, сервис продолжит проверять значение флага `tracing` раз в `FeatureFlagUpdateIntervalSeconds` секунд.
Если же при обновлении недоступен gitlab или не прошла десериализация, то сервис попытается снова через `FeatureFlagUpdateIntervalOnFailureSeconds` секунд.

Переменные `FeatureFlagUpdateIntervalSeconds` и `FeatureFlagUpdateIntervalOnFailureSeconds` можно задать в `appsettings`:
```
  "Tracing": {
    "FeatureFlagUpdateIntervalSeconds": 600,
    "FeatureFlagUpdateIntervalOnFailureSeconds": 20
  }
```
Значения по умолчанию `FeatureFlagUpdateIntervalSeconds` = 60, `FeatureFlagUpdateIntervalOnFailureSeconds` = 10