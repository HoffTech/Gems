# Gems.Swagger
Библиотека для поддключения Swagger.  

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#УстановкаИНастройка)

# Установка и настройка
- Установите нугет пакет Gems.Swagger через менеджер пакетов.

Настройки в appsettings.json
```json
 "Swagger": {
    "EnableAuthority": false/true, - отключить/включить использование авторизации, в контроллерах использовать [Authorize]  
    "SwaggerKey": "swagger_key", - oauth2 clientId по умолчанию
    "SwaggerName": "swagger_name", - описание, которое отображается в раскрывающемся списке выбора документа.
    "SwaggerSchema": "swagger_schema", - добавляется ко всем операциям в секцию security.oauth2
    "GitLabSwaggerPrefix": "prefix" - это значение задается в файле .gitlab-ci.yml в переменной CI_PROJECT_URL_PATH
  },
```
- В конфигурации сервисов добавьте строки:
```csharp
services.AddSwagger(this.Configuration, typeof(ValidationResultViewModel), typeof(GenericErrorViewModel));
```
Где класс ValidationResultViewModel - это модель ответа (Response 400), если произошли ошибки валидации. Добавляется автоматически ко всем операциям.\
Класс GenericErrorViewModel  - это модель ответа (Response 500), если произошли другие ошибки.  Добавляется автоматически ко всем операциям.\
PS: Когда включен флаг EnableAuthority, то  ко всем операциям добавляется автоматически ответы Response 401 и Response 403.

- В конфигурации приложения добавьте строки после app.UseRouting():
```csharp
app.UseSwaggerApi(this.Configuration);
```
Если флаг EnableAuthority установлен в true, то необходимо добавить настройки для авторизации (см. описание работы с библиотекой Gems.Authentication).