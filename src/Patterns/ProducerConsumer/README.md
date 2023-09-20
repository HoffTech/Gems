# Gems.Patterns

Реализация паттернов

# Содержание

* [Producer-consumer](#producer-consumer)

# Producer-consumer

Смысл паттерна в том, что один поток производит данные, и параллельно этому один или несколько потоков потребляют их. В основе данной реализации паттерна исопльзован класс BlockingCollection<T>.

1. Установите пакет Gems.Patterns в проекте.

2. Добавьте для более гибкой настройки следующие строки в appsettings:
```json
"ProducerConsumerOptions": {
      "MaxAttempts": 3, //количество попыток выполнения задачи чтения консамером
      "DelayBetweenAttemptsInMilliseconds": 3000 //временной интервал между попытками чтения консамером
  },
```
3. Библиотека добавляет метод расширения AddProducerConsumerPattern, в качестве параметра необходимо передать лямбда выражение, которое устанавливает значение опций:
```csharp
services.AddProducerConsumerPattern(options =>
{
    options.MaxAttempts = ...;
});
```

4. Создайте экземпляр класса BaseProducerConsumer<TTaskInfo>, где TTaskInfo - тип описывающий ваши бизнес данные. В конструктор передайте следующие параметры:
```csharp
IOptions<ProducerConsumerOptions> options, // параметры
Func<ProducerConsumer<TTaskInfo>, CancellationToken, Task> produceInternalAsync, // функция по генерации данных
Func<TTaskInfo, CancellationToken, Task> consumeInternalAsync, // функция по потреблению данных
Func<Exception, Task> onErrorAction = null, // функция выполняемая в случае возикновения ошибки в консамере
List<Type> exceptionHandleTypes = null // список исключений, по которым будут осуществляться повторные попытки синхронизации
```
Функция produceInternalAsync вызывается продюсером, добавьте в нее ваш алгоритм генерации данных. consumeInternalAsync - вызывается консамерами, добавьте в нее ваш алгоритм для потребления данных.
Для выполнения операции передачи данных между продюсером и консамером необходимо вызвать метод  
```csharp
StartAsync(CancellationToken cancellationToken)
```
