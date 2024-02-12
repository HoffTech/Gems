# Инструкция по применению библиотеки Gems.MessageBrokers.Kafka

Данная библиотека существенно упрощает отправку/чтение сообщений из топиков Кафки. Для обработки прочитанных сообщений используются команды паттерна CQRS, поэтому библиотека Mediatr обязательна к подключению к проекту.

Для работы с библиотекой необходимо:

1. Подключить к проекту библиотеку MediatR, при необходимости FluentValidation;

2. Подключить библиотеку Gems.MessageBrokers.Kafka;

3. Добавить настройки в `appsettings.json`:

```json
{
  "KafkaConfiguration": 
  {
    "BootstrapServers": "", // список серверов через запятую
    "SecurityProtocol": "0", // протокол безопасности - PLAINTEXT
    "SaslMechanism": "1", // механизм аутентификации - PLAIN
    "SaslUsername": "", // логин пользователя
    "SaslPassword": "", // пароль пользователя
    "SchemaRegistryUrl": "", // ссылка на ресурс со схемами
    "Producers": // списко продюсеров 
    {
      "producertopicname1": // имя вашего конкретного топика 
      {
        "ClientId": "ClientId" // идентификатор клиента-продюсера (указываете произвольный, но обычно как названия вашего приложения)
      },
      "producertopicname2": // имя вашего конкретного топика 
      {
        "ClientId": "ClientId" // идентификатор клиента-продюсера (указываете произвольный, но обычно как названия вашего приложения)
      }
    },
    "Consumers": // список консьюмеров 
    {
      "consumertopicname1": // имя вашего конкретного топика 
      {
        "GroupId": "GroupId", // идентификатор клиента-консьюмера (указываете произвольный, но обычно как названия вашего приложения). Если реплик несколько, то будет зарегистрирована группа консьюмеров 
        "AutoOffsetReset": 1,           // Earliest, в случае если будет не понятно какое сообщение обрабатывать из топика, начнется с первого. Такое может произойти из-за сбоев и ребалансировки партиций. 
        "EnableAutoCommit" : true,      // будет ли делаться автоматический коммит после обработки сообщения, даже если безуспешно. По умолчанию true. Рекомендуется устанавливать в false, тогда коммится будет только после успешной обработки.
        "EnableAutoOffsetStore": true,  // будет ли делаться предварительное сохранение коммита в памяти. По умолчанию true. Рекомендуется устанавливать в false, тогда коммится будет сразу
        "EnableRetry": false,           // будут ли делаться повторные обработки сообщений. Если false, то раздел RetryAttempts будет игнорироваться.  
        "RetryAttempts": [              // настройка повторных запусков обработки сообщения. По умолчанию 5 минут.         
          {
            "DelayInMilliseconds": 5000,  // запуск повторной обработки через 5 секунд
            "CountAttempts": 3            // количество повторений. По умолчанию 1
          },
          {
            "DelayInMilliseconds": 15000,  // запуск повторной обработки через 15 секунд
            "CountAttempts": 3            // количество повторений. По умолчанию 1
          },
          // ... Some other RetryAttempts settings 
          {
            "DelayInMilliseconds": 3600000,  // запуск повторной обработки через 1 час
            "CountAttempts": 1000            // количество повторений. Данное значение не важно. Повторения для последней настройки будет бесконечно 
          }
        ]
      },
      "consumertopicname2": // имя вашего конкретного топика 
      {
        "GroupId": "GroupId", // идентификатор клиента-консьюмера (указываете произвольный, но обычно как названия вашего приложения). Если реплик несколько, то будет зарегистрирована группа консьюмеров
        "AutoOffsetReset": 1 // Earliest, в случае если будет не понятно какое сообщение обрабатывать из топика, начнется с первого. Такое может произойти из-за сбоев и ребалансировки партиций
        "EnableAutoCommit" : true, // будет ли делаться автоматический коммит после обработки сообщения, даже если безуспешно. По умолчанию true. Рекомендуется устанавливать в false, тогда коммится будет только после успешной обработки.
        "EnableAutoOffsetStore": true // будет ли делаться предварительное сохранение коммита в памяти. По умолчанию true. Рекомендуется устанавливать в false, тогда коммится будет сразу
      }
    }
  }
}
```    

4. Зарегистрировать сервисы в Startup:
```csharp
services.AddProducers(); 
services.AddConsumers();
```
5. Реализовать классы команд для библиотеки MediatR:
      ```csharp
        public class TestCommand : IRequest
        {
        }
      ```
   Класс команды должен иметь публичный конструктор без параметров, в противном случае при старте приложения будут возникать исключения вида   violates the constraint of type 'THandlerCommand'.
  
5. Реализовать классы обработчики команд для библиотеки MediatR, сами классы пометить кастомным свойством `ConsumerListenerProperty`, например так:
      ```csharp
        [ConsumerListenerProperty("consumertopicname1")]
        public class TestCommandHandler : IRequestHandler<TestCommand>
      ```
    Где consumertopicname1 - это читаемый топик (тот что в appsettings.json). Логику обработки прочитанных сообщений следует размещать в данном классе.
	
6. Отправка сообщений в топик осуществляется через метод `ProduceAsync<TKey, TValue>(string topic, TKey key, TValue message)` интерфейса `IMessageProducer`.:
```csharp
public class SomeClass
{
    private readonly IMessageProducer messageProducer;

    public SomeClass(IMessageProducer messageProducer)
    {
        this.messageProducer = messageProducer;
    }

    public async Task SomeMethod()
    {
        await this.messageProducer.ProduceAsync(
            "consumertopicname1",
            "some_key",
            new SomeDto { SomeValue = "<some_value>" });
    }
}
```
В метод ProduceAsync передаются топик, тип ключа и тип сообщения