# Gems

- [Gems](#gems)
- [Введение](#введение)
- [Архитектура приложения](#архитектура-приложения)
- [Работа с данными](#работа-с-данными)
- [Http запросы](#http-запросы)
- [Логи](#логи)
- [Метрики](#метрики)
      - [Реализация Prometheus](#реализация-prometheus)
- [Работа с задачами](#работа-с-задачами)
- [Брокер сообщений](#брокер-сообщений)
      - [Реализация Kafka](#реализация-kafka)
- [Планировщик заданий](#планировщик-заданий)
      - [Реализация Quartz](#реализация-quartz)
      - [Реализация Hangfire](#реализация-hangfire)
- [Сериализация](#сериализация)
- [Инициализация swagger](#инициализация-swagger)
- [Аутентификация по протоколу OpenId Connect](#аутентификация-по-протоколу-openid-connect)
- [Кэширование](#кэширование)
- [Проверка доступности сервиса](#проверка-доступности-сервиса)
- [Настройки](#настройки)
- [Работа с задачами](#работа-с-задачами-1)
- [OpenTelemetry](#opentelemetry)
- [Утилитарные библиотеки](#утилитарные-библиотеки)
- [Управление версиями gems библиотек](#управление-версиями-gems-библиотек)

# Введение
  На данной странице описаны все библиотеки, которые на данный момент реализованы. Все библиотеки сгруппированы по категориям или назначению. При переходе по ссылке в можно подробнее ознакомиться с назначением и способом применения каждой из библиотек.
  
# Архитектура приложения
- [Gems.Mvc](/src/Mvc/Mvc)
- [Gems.BusinessRules](/src/BusinessRules/BusinessRules)
- [Gems.DomainEvents](/src/DomainEvents/DomainEvents)
- [Gems.Patterns.Outbox](/src/Patterns/Outbox)
- [Gems.Patterns.ProducerConsumer](/src/Patterns/ProducerConsumer)
- [Gems.Patterns.SyncTables](/src/Patterns/SyncTables)

# Работа с данными
- [Gems.Data](/src/Data/Data)
- [Gems.Data.Npgsql](/src/Data/Npgsql)
- [Gems.Data.SqlServer](/src/Data/SqlServer)
- [Gems.Data.MySql](/src/Data/MySql)

# Http запросы
- [Gems.Http](/src/Http/Http)

# Логи
- [Gems.Logging.Security](/src/Logging/Security)
- [Gems.Logging.Mvc](/src/Logging/Mvc)
- [Gems.Logging.Serilog](/src/Logging/Serilog)

# Метрики
- [Gems.Metrics](/src/Metrics/Metrics)
- [Gems.Metrics.Data](/src/Metrics/Data)
- [Gems.Metrics.Http](/src/Metrics/Http)

#### Реализация Prometheus
- [Gems.Metrics.Prometheus](/src/Metrics/Prometheus)

# Работа с задачами
- [Gems.Tasks](/src/Tasks/Tasks)

# Брокер сообщений
- [Gems.MessageBrokers](/src/MessageBrokers/MessageBrokers)
#### Реализация Kafka
- [Gems.MessageBrokers.Kafka](/src/MessageBrokers/Kafka)

# Планировщик заданий
- [Gems.Jobs](/src/Jobs/Jobs)
#### Реализация Quartz
- [Gems.Jobs.Quartz](/src/Jobs/Quartz)
#### Реализация Hangfire
- [Gems.Jobs.Hangfire](/src/Jobs/Hangfire)

# Сериализация
- [Gems.Text.Json](/src/Text/Json)

# Инициализация swagger
- [Gems.Swagger](/src/Swagger/Swagger)

# Аутентификация по протоколу OpenId Connect
- [Gems.Authentication](/src/Authentication/Authentication)

# Кэширование
- [Gems.Caching](/src/Caching/Caching)

# Проверка доступности сервиса
- [Gems.HealthChecks](/src/HealthChecks/HealthChecks)

# Настройки
- [Gems.Settings.Gitlab](/src/Settings/Gitlab)
- [Gems.FeatureToggle](/src/FeatureToggle/FeatureToggle)

# Работа с задачами
- [Gems.Tasks](/src/Tasks/Tasks)

# OpenTelemetry
- [Gems.OpenTelemety](/src/OpenTelemetry/OpenTelemetry)
- [Gems.OpenTelemety.Mvc](/src/OpenTelemetry/Mvc)

# Утилитарные библиотеки
- [Gems.Utils](/src/Utils/Utils)
- [Gems.Linq](/src/Linq/Linq)
- [Gems.IO](/src/IO/IO)
- [Gems.Extensions.DependencyInjection](/src/Extensions/DependencyInjection)

# Управление версиями gems библиотек
Управление версиями gems библиотек описано в [документе](docs/VersioningRules.md)