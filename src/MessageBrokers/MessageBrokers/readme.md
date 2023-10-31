# Gems.Jobs
Содержит общие классы и интерфейсы для работы с брокерами сообщений.

Библиотека предназначена для следующих сред выполнения (и новее):

.NET 6.0

# Содержание

* [Атрибут ConsumerListenerProperty](#атрибут-ConsumerListenerProperty)

# Атрибут ConsumerListenerProperty
Обработчик команды или запроса необходимо предварять атрибутом ConsumerListenerProperty.
```csharp
[ConsumerListenerProperty("consumertopicname1", KeyType = typeof(string), ValueType = typeof(LoadContractorsCommand), NeedParseJsonFromString = false))]
public class CheckPaymentCommandHandler : IRequestHandler<CheckPaymentCommand>
{
    // ...
}
```
Где первый параметр является обязательным- это читаемый топик.  
Остальные параметры необязательные: 
- KeyType - это тип ключа (по умолчанию тип строки)
- ValueType - тип значения (по умолчанию тип команды/запроса)
- NeedParseJsonFromString - необходимо ли парсить значение из строки (по умолчанию false)

NeedParseJsonFromString имеет смысл передавать, если значение в топике не имеет Magic Byte в начале сообщения (json-а), отвечающее за формат сообщения. Для json-а Magic Byte - это последовательность из нулевых символов \0\0\0\0. 

Если Magic Byte не установлен, то будет бросаться ошибка, в тексте которой будет: "Confluent Schema Registry framing. Magic byte was 123, expecting 0". Тогда NeedParseJsonFromString следует установить в true.  
NeedParseJsonFromString можно установить в true, только если ValueType равен типу команды или запроса.

Второй конструктор:
```csharp
[ConsumerListenerProperty(typeof(string), typeof(LoadContractorsCommand), "consumertopicname1")]
public class CheckPaymentCommandHandler : IRequestHandler<CheckPaymentCommand>
{
    // ...
}
```
Где в качестве параметров последовательно передаются тип ключа, тип значнеия, читаемый топик.

Далее в зависимости от типа брокера сообщений необходимо произвести соответсвующие настройки в appsettings и Startup.  
Смотрите описание в [Gems.MessageBrokers.Kafka](/src/MessageBrokers/Kafka).