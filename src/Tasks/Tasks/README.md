# Gems.Tasks

Вспомогательная библиотека для работы с задачами(Task).

# Содержание

* [AsyncAwaiter](#asyncawaiter)
* [TaskExtensions](#taskextensions)
* [ParallelExecutor](#parallelexecutor)
* [AsyncDecorator](#asyncdecorator)

# AsyncAwaiter
Добавляет возможность безопасно ждать завершения задач, требующих ограниченного доступа. Использует безопасный семафор, чтобы предотвратить любую возможность тупиковой ситуации. Содержит два статических метода:
* __Task<T> AwaitResultAsync<T>(_string key, Func<Task<T>> task, int maxAccessCount = 1_)__ - ожидает завершения любых невыполненных задач, которые обращаются к одному и тому же ключу, затем запускают данную задачу, возвращая её значение.
* __Task AwaitAsync(_string key, Func<Task> task, int maxAccessCount = 1_)__ - ожидает завершения любых невыполненных задач, которые обращаются к одному и тому же ключу, затем запускает данную задачу
* Примеры:
    ```csharp
    public Task<Response> Method(CancellationToken cancellationToken)
    {
        return AsyncAwaiter.AwaitResultAsync<Response>("some_Key", 
            () => 
            {
                return ProcessingLines(cancellationToken);
            }
        )
    }
    ```

    ```csharp
    public Task Method(CancellationToken cancellationToken)
    {
        return AsyncAwaiter.AwaitResultAsync(
            "some_Key", 
            () => ProcessingLines(cancellationToken);
        )
    }
    ```


# TaskExtensions
Содержит расширения для работы с задачами(Tasks):

* SafeFireAndForget(_this Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null_) - статический метод для безопасного выполнения задачи без ожидания результата.<br/>
Входные параметры:
    * bool continueOnCapturedContext, по умолчанию true - если true, продолжение задачи будет выполняться в захваченном контексте и исходном потоке. Если false - продолжение будет выполняться в другом контексте и может быть выполнено в другом потоке.
    * Action<Exception> onException, по умолчанию null - action, который необходимо выполнить в случае исключения в выполняемой задаче. Если onException null, то исключение из выполняемой задачи будет проброшено дальше.
* Пример:
    ```csharp
    try
    {
        ProcessingLines().SafeFireAndForget(false);
    }
    catch
    {
        // обработка ошибок
    }
    ```


# ParallelExecutor
Добавляет возможность итеративно распараллелить выполнение задач по обработке массива данных.
* __Task SyncDataAsync(_int totalRows, int maxSemaphoreTasks, List<Type> exceptionHandleTypes, Func<int, int, CancellationToken, Task> action, CancellationToken cancellationToken, int maxTakeSize = 1000, maxAttempts = 10_)__ - параллельно выполняет синхронизацию данных из БД источника в БД получателя. Позволяет настроить ограничение по одновременному обращению к ресурсу, используя во внутренней реализации метод AsyncAwaiter.AwaitAsync, а также позволяет реализовать попытки повторного обращения к ресурсу в случае возникновения исключений.<br/>
  Входные параметры:
  * int totalRows - общее колличество элементов для обработки.
  * int maxSemaphoreTasks - ограничение на максимальное колличество одновременно выполненяемых.
  * List<Type> exceptionHandleTypes - список исключений, по которым будут осуществляться повторные попытки синхронизации.
  * Func<int, int, CancellationToken, Task> action - действие для задачи (skip, take, CancellationToken).
  * ILogger logger - логгер для логирования ошибок во время синхронизации.
  * CancellationToken cancellationToken - токен отмены.
  * int maxTakeSize, по умолчанию 1000 - ограничение на максимальное колличество строк для выборки.
  * int maxAttempts, по умолчанию 10 - ограничение на максимальное колличество попыток связанных с БД при возникновении ошибок при синхронизации.
* Пример:
    ```csharp
    public Task Method(CancellationToken cancellationToken)
    {
        await ParallelExecutor.SyncDataAsync(
                totalRows: 3_714_238,
                maxSemaphoreTasks: 15,
                exceptionHandleTypes: new List<Type> { SqlException, PostgresException },
                (skip, take, cancellationToken) =>
            {
                var data = this.getDataService.GetDataAsync(skip, take, cancellationToken);
                this.writeDataService.WritaDataAsync(data);
                return Task.CompletedTask;
            },
            logger,
            cancellationToken: cancellationToken,
            maxTakeSize: 1000,
            maxAttempts: 10);
    }
    ```

  
# AsyncDecorator
Содержит декораторы с различными сценариями для асинхронных методов.
* __Task<TReturn> DurableExecuteAsync(_Func<Task<TReturn>> func, CancellationToken cancellationToken, TimeSpan delayBetweenAttempts = default, int maxAttempts = 10, List<Type> exceptionHandleTypes = null, Func<Exception, Task> onErrorAction = null_)__ - метод декорирующий асинхронную задачу, учитывающий максимальное кол-во попыток на выполнение в случае возникновения исключений.<br/>
  * Входные параметры:
  * Func<CancellationToken, Task<TReturn>> func - действие для задачи.
  * CancellationToken cancellationToken - токен отмены.
  * TimeSpan delayBetweenAttempts, по умолчанию default - временной интервал между попытками.
  * int maxAttempts, по умолчанию 10 - ограничение на максимальное колличество попыток выполнения задачи.
  * List<Type> exceptionHandleTypes, по умолчанию null - список исключений, по которым будут осуществляться повторные попытки синхронизации.
  * Func<Exception, Task> onErrorAction, по умолчанию null - действие в случае возникновения ошибки.
  * Пример:
    ```csharp
    public Task Method(CancellationToken cancellationToken)
    {
        await AsyncDecorator.DurableExecuteAsync(
            async (cancellationToken) =>
            {
                await fakeService.DoFakeWorkAsync(cancellationToken).ConfigureAwait(false);
                return Task.CompletedTask;
            },
            cancellationToken,
            TimeSpan.FromMilliseconds(500),
            maxAttempts: 10,
            new List<Type> { typeof(TestException) },
            ex =>
            {
                logger.LogError(ex, "Test error log");
                return Task.CompletedTask;
            })
    }
    ```
