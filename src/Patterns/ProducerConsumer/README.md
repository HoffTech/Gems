# Gems.Patterns

Реализация паттернов

# Содержание

* [Producer-consumer](#producer-consumer)

# Producer-consumer
**[Пример кода](/src/Patterns/ProducerConsumer/samples/Gems.Patterns.ProducerConsumer.SampleUsing)**

Смысл паттерна в том, что один поток производит данные, и параллельно этому один или несколько потоков потребляют их. В основе данной реализации паттерна использован класс BlockingCollection<T>.