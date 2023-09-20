# Инструкция по применению библиотеки Gems.MessageBrokers.Kafka

Данная библиотека существенно упрощает отправку/чтение сообщений из топиков Кафки. Для обработки прочитанных сообщений используются команды паттерна CQRS, поэтому библиотека Mediatr обязательна к подключению к проекту.

Для работы с библиотекой необходимо:

1. Подключить к проекту библиотеку MediatR, при необходимости FluentValidation;

2. Подключить библиотеку Gems.Kafka.Hosting;

3. Создать записи в файлах `appsettings.json` в раздел `KafkaConfiguration`:
    
      ```
    "BootstrapServers": "kafka-s-001.kifr-ru.local:9092,kafka-s-001.kifr-ru.local:9093,kafka-s-001.kifr-ru.local:9094",
    "SecurityProtocol": "0", //PLAINTEXT
    "SaslMechanism": "1", //PLAIN
    "SaslUsername": "",
    "SaslPassword": "",
    "SchemaRegistryUrl": "http://kafka-s-001:8081"
      ```
    где `BootstrapServers` - список серверов через запятую, 
    `SecurityProtocol` - протокол безопасности, 
    `SaslMechanism` - механизм аутентификации, 
    `SaslUsername` - логин пользователя, 
    `SaslPassword` - пароль пользователя, 
    `SchemaRegistryUrl` - ссылка на ресурс со схемами;

    Для настройки продюсеров создать структуру вида:
      ```
    "Producers": {
    "producertopicname1": {
        "ClientId": "ClientId"
    },
    "producertopicname2": {
        "ClientId": "ClientId"
    }
    }
      ```
    где `producertopicname1` - имя вашего конкретного топика,
    `ClientId` - идентификатор клиента-продюсера;

    Для настройки консамеров создать структуру вида:
      ```
        "Consumers": {
        "consumertopicname1": {
            "GroupId": "GroupId",
            "AutoOffsetReset": 1 //Earliest
        },
        "consumertopicname2": {
            "GroupId": "GroupId",
            "AutoOffsetReset": 1 //Earliest
        },
      ```
    где `consumertopicname1` - имя вашего конкретного топика,
    `GroupId` - идентификатор группы подписчиков,
    `AutoOffsetReset` - смещение сообщения в топике по умолчанию;
	
4. Реализовать классы команд для библиотеки MediatR, класс должен реализовать `IConsumerRequest<T>`, где T - тип значения сообщения полученного из топика, например:
      ```
        public class TestRequest1 : IRequest<string>, IConsumerRequest<string>
        {
            public string Value { get; set; }
        }
      ```
   Класс команды должен иметь публичный конструктор без параметров, в противном случае при старте приложения будут возникать исключения вида   violates the constraint of type 'THandlerCommand'.
  
5. Реализовать классы обработчики команд для библиотеки MediatR, сами классы пометить кастомным свойством `ConsumerListenerProperty`, например так:
      ```
        [ConsumerListenerProperty(typeof(string), typeof(string), "consumertopicname1")]
        public class TestHandler_String : IRequestHandler<Request, string>
      ```
    Свойству в качестве параметров передаются тип ключа, тип значнеия, читаемый топик. Логику обработки прочитанных сообщений следует размещать в данном классе.
	
6. Прописать вызов методов `services.AddProducers()` и `services.AddConsumers()` в методе регистрации зависимостей.

7. Для отправки сообщений в топик необходимо вызвать метод `ProduceAsync<TKey, TValue>(string topic, TKey key, TValue message)` интерфейса `IMessageProducer`. В метод передаются топик, тип ключа и тип сообщения.