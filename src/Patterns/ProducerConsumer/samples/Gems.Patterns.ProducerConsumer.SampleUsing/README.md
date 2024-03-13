# Пример использования паттерна Producer-consumer

Добавьте для более гибкой настройки следующие строки в appsettings:

```json
"ProducerConsumerOptions": {
"MaxAttempts": 3, //количество попыток выполнения задачи чтения консамером
"DelayBetweenAttemptsInMilliseconds": 3000 //временной интервал между попытками чтения консамером
},
```

Библиотека добавляет метод расширения AddProducerConsumerPattern, в качестве параметра необходимо передать лямбда выражение, которое устанавливает значение опций:
```csharp
services.AddProducerConsumerPattern(options =>
{
    options.MaxAttempts = ...;
});
```

Создайте экземпляр класса BaseProducerConsumer<TTaskInfo>, где TTaskInfo - тип описывающий ваши бизнес данные. В конструктор передайте следующие параметры:
```csharp
IOptions<ProducerConsumerOptions> options, // параметры
Func<ProducerConsumer<TTaskInfo>, CancellationToken, Task> produceInternalAsync, // функция по генерации данных
Func<TTaskInfo, CancellationToken, Task> consumeInternalAsync, // функция по потреблению данных
Func<Exception, Task> onErrorAction = null, // функция выполняемая в случае возикновения ошибки в консамере
List<Type> exceptionHandleTypes = null // список исключений, по которым будут осуществляться повторные попытки синхронизации
```

Пример функции по потреблению данных. 
Следует обратить внимание на вызов метода `producerConsumer.AddTaskInfo`, который добавляет задание

```csharp
    private static async Task StartProduceAsync(
        ProducerConsumer<ExternalPerson> producerConsumer,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection("SOME CONNECTION STRING");
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = new SqlCommand("SELECT PersonId FROM dbo.PERSONS", connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (!reader.HasRows)
        {
            return;
        }

        var colPersonId = reader.GetOrdinal("PersonId");

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            producerConsumer.AddTaskInfo(new ExternalPerson
            {
                PersonId = reader.ReadData(colPersonId, reader.GetGuid),
            });
        }
    }
```

Для выполнения операции передачи данных между продюсером и консамером необходимо вызвать метод `StartAsync`
```csharp
    public Task Handle(SyncPersonsCommand request, CancellationToken cancellationToken)
    {
        var producerConsumer = new ProducerConsumer<ExternalPerson>(this.producerConsumerOptions, StartProduceAsync, StartConsumerAsync);

        return producerConsumer.StartAsync(cancellationToken);
    }
```