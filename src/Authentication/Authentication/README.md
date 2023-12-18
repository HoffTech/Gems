# Gems.Authentication

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#УстановкаИНастройка)
* [JWT аутентификация](#JWTАутентификация)
* [Keycloak аутентификация](#KeycloakАутентификация)
* [Использование пайплайнов](#использование-пайплайнов)
* [AuthorizationBehavior](#AuthorizationBehavior)

# Установка и настройка <a name="УстановкаИНастройка"></a>
- Установите нугет пакет Gems.Authentication через менеджер пакетов.

# JWT аутентификация <a name="JWTАутентификация"></a>

Реализация подключения аутентификации по протоколу OpenId Connect(https://openid.net/connect/) на основе JWT токена.

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

# Keycloak аутентификация <a name="KeycloakАутентификация"></a>
Реализация подключения keycloak аутентификации по протоколу OpenId Connect(https://openid.net/connect/) посредством code flow потока (https://www.keycloak.org/docs/latest/securing_apps/#_oidc).
Данный тип аутентификации предназначен для web, нативных и мобильных приложений, в которые возможно встроить пользовательский агент. 

Настройки в appsettings.json
```json
  "KeycloakAuthOptions": {
    "CookieOptions": {
        "Name": "keycloak.cookie", - имя cookies
        "MaxAge": 60 - время жизни cookies в минутах
    },
    "OpenIdConnectOptions": {
        "ClientId": "localclient", - имя вашего клинета, настраивается в keycloak
        "ClientSecret": "{секрет клинета}", - настраивается в keycloak
        "MetadataAddress": "http://localhost:8080/realms/localrealm/.well-known/openid-configuration", - discovery endpoint, нужен для получения метаданных, формируется так: [путь до рилма, в котором зарегистрирован ваш клиент]/.well-known/openid-configuration
        "Authority": "http://localhost:8080/realms/localrealm", - endpoint рилма, в котором зарегистрирован ваш клиент
        "SignedOutRedirectUri": "https://localhost:5001/Auth", - endpoint на который редиректится пользователь после выхода из акаунта
        "TokenValidationParameter": { - параметры валидации токена
            "NameClaimType": "name", - тип утверждения, который используется для определения того, какие утверждения предоставляют значение для свойства Name
        }
    }
  },
```
- В конфигурации сервисов добавьте строки:
```csharp
services.AddKeycloakAuthentication(this.Configuration);
```

- В конфигурации приложения добавьте строки после app.UseRouting():
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

# Использование пайплайнов
Взаимен использования _UseAuthentication()_ и _UseAuthorization()_ предусмотрен пайплайн _AuthorizationBehavior_ для проверки аутентификации и авторизации.



Регистрация пайплайна:
```csharp
services.AddPipeline(typeof(AuthorizationBehavior<,>));
```

# AuthorizationBehavior
Пайплайн AuthorizationBehavior производит проверки пользователя, такие как:
-  проверка пользователя на подлинность, выбрасывает _AuthenticationException_
-  проверка пользователя на соответствие одной из Роли переданной в массиве при имплементации интерфейса, выбрасывает _ForbiddenAccessException_
_- (если ни одной роли не указано, то производит только проверку на подлинность)._
Если проверка не пройдена, выбрасывает UnauthorizedAccessException.
- При некорректном использовании Behavior, вне доступа HttpContext, выбрасывает _InvalidOperationException_

Пример добавления пайплайна к обработчику для проверки пользователя на подлинность:
```csharp
    public class DoCommand : IRequest, IRequestAuthorization
    {
        // ...
    }
```

Пример добавления пайплайна к обработчику для проверки на подлинность и доступа для ролей:
```csharp
    public class DoCommand : IRequest, IRequestAuthorization
    {
        // ...

        public IEnumerable<string> GetRoles()
        {
            return new[] { "Admin", "Manager", "User" };
        }
    }
```