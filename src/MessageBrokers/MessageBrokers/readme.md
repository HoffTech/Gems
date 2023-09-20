# Gems.Jobs
Содержит общие классы и интерфейсы для работы с брокерами сообщений.

Библиотека предназначена для следующих сред выполнения (и новее):

.NET 6.0

# Содержание

* [Атрибут ConsumerListenerProperty](#атрибут-ConsumerListenerProperty)

# Атрибут ConsumerListenerProperty
Обработчик команды или запроса необходимо предварять атрибутом ConsumerListenerProperty.
```csharp
[ConsumerListenerProperty(typeof(string), typeof(string), "consumertopicname1")]
public class CheckPaymentCommandHandler : IRequestHandler<CheckPaymentCommand>
{
    // ...
}
```
Свойству в качестве параметров передаются тип ключа, тип значнеия, читаемый топик.

Далее в зависимости от типа брокера сообщений необходимо произвести соответсвующие настройки в appsettings и Startup.  
Смотрите описание в [Gems.MessageBrokers.Kafka](/src/MessageBrokers/Kafka).