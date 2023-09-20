# Gems.Caching
Библиотека содержит расширения для работы с кэшом:
- расширение для инициализации кэша в памяти или в redis.
- расширение для работы с IDistributedCache.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка-и-настройка)
* [Расширение IDistributedCache](#расширение-idistributedcache)
# Установка и настройка
- Установите нугет пакет Gems.Caching через менеджер пакетов.

Настройки в appsettings.json
```json
"ConnectionStrings": {
  "Redis": "<connection_string>"              // по умолчанию для redis
},
```
- В конфигурации сервисов добавьте строки:
```csharp
// регистрация кэша
services.AddDistributedCache(this.Configuration); // если строка подключения к redis пустая, то используется кэш в памяти
```

# Расширение IDistributedCache

Библиотека добавляет метод, позволяющий получить или установить кэш.
```csharp
this.distributedCache.GetOrCreateAsync(cacheKey, () => GetSomeDataAsync(cancellationToken), cancellationToken)

// IDistributedCache необходимо передать в конструктор
// в библиотеке присутсвует перегруженная версия для данного метода: GetOrCreate
```