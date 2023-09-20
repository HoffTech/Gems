# Gems.Authentication
Реализация подключения аутентификации по протоколу OpenId Connect(https://openid.net/connect/) на основе JWT токена.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#УстановкаИНастройка)

# Установка и настройка
- Установите нугет пакет Gems.Authentication через менеджер пакетов.

Настройки в appsettings.json
```json
"AD": {
    "Authority": "https://fs.example.ru/adfs",
    "ValidIssuer": "http://fs.example.ru/adfs/services/trust",
    "ClientId": "unique_client_id", - уникальное имя, заводится в AD
    "CertFileName": "file_name.cer", - файл с публичным ключём
},
```
- В конфигурации сервисов добавьте строки:
```csharp
services.AddJwtAuthentication(this.Configuration);
```

- В конфигурации приложения добавьте строки после app.UseRouting():
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Для работы c авторизацией, необходимо, чтобы в AD зарегистрировали redirect_ url. Пример: https://api-dev.kifr-ru.local/{GitLabSwaggerPrefix}/swagger/oauth2-redirect.html