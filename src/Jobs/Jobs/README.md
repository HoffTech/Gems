# Gems.Jobs
Содержит общие классы и интерфейсы для работы с планировщиками заданий.

Библиотека предназначена для следующих сред выполнения (и новее):

.NET 6.0

# Содержание

* [Атрибут JobHandlerAttribute](#атрибут-JobHandlerAttribute)
* [Интерфейс IRequestReFireJobOnFailed](#интерфейс-IRequestReFireJobOnFailed)

# Атрибут JobHandlerAttribute
Обработчик команды или запроса необходимо предварять атрибутом JobHandlerAttribute.
Конструктор атрибута принимает один аргумент - имя задания.
```csharp
[JobHandler("CheckPayment")]
public class CheckPaymentCommandHandler : IRequestHandler<CheckPaymentCommand>
{
    // ...
}
```
Далее в зависимости от типа планировщика необходимо произвести соответсвующие настройки в appsettings и Startup.  
Смотрите описание в [Gems.Jobs.Quartz](/src/Jobs/Quartz/README.md) или в [Gems.Jobs.Hangfire](/src/Jobs/Hangfire/README.md)

# Интерфейс IRequestReFireJobOnFailed
Позволяет повторно запустить задачу после задержки в случае возникновения исключения.
Унаследуйте команду или запрос от интерфейса IRequestReFireJobOnFailed.  
Смотрите описание в [Gems.Jobs.Quartz](/src/Jobs/Quartz/README.md) или в [Gems.Jobs.Hangfire](/src/Jobs/Hangfire/README.md)