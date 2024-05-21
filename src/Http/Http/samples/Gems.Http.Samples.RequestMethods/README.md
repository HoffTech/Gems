# Типы запросов BaseClientService

### Типы запросов
1.  _GetAsync_ - запрос получения данных
     - _GetStringAsync_ - запрос получение данных в виде строки
     - _GetStreamAsync_ - запрос получения данных в виде потока
     - _GetByteArrayAsync_ - запрос получения данных в виде массива байтов
3. _PostAsync_ - запрос отправки данных для добавления
3. _PutAsync_ - запрос отправки данных для добавления или обновления 
4. _PatchAsync_ - запрос отправки данных для обновления
5. _DeleteAsync_ - запрос удаления данных

### Параметры
1. `TemplateUri templateUri` - шаблонный Uri
2. Параметры данных
   - `object queryString` - данные в виде queryString
   - `object requestData` - данные в виде requestBody
3. `IDictionary<string, string> headers` - заголовки
4. `CancellationToken cancellationToken` - токен отмены

### Примеры
1. _GetAsync_ - запрос получения данных
```csharp
this.defaultClientService.GetAsync<PaymentStatus>(
    "api/payments/{id}/status".ToTemplateUri(query.PaymentId),
    cancellationToken);
```
2. _GetStringAsync_ - запрос получение данных в виде строки
```csharp
this.defaultClientService.GetStringAsync(
    "api/payments/{id}/bill".ToTemplateUri(query.BillId),
    cancellationToken);
```
3. _GetStreamAsync_ - запрос получения данных в виде потока
```csharp
this.defaultClientService.GetStreamAsync(
    "api/payments/{id}/document".ToTemplateUri(query.PaymentId),
    cancellationToken)
```
4. _GetByteArrayAsync_ - запрос получения данных в виде массива байтов
```csharp
this.defaultClientService.GetByteArrayAsync(
    "api/v1/payments/{id}/invoice".ToTemplateUri(query.InvoiceId),
    cancellationToken)
```
5. _PostAsync_ - запрос отправки данных для добавления
```csharp
this.defaultClientService.PostAsync<string>(
    "api/payments".ToTemplateUri(),
    new
    {
        command.ClientId,
        command.Amount
    },
    cancellationToken)
```
6. _PutAsync_ - запрос отправки данных для добавления или обновления
```csharp
this.defaultClientService.PutAsync<string>(
    "api/payments".ToTemplateUri(),
    new
    {
        command.ClientId,
        command.Amount
    },
    cancellationToken);
```
7. _PatchAsync_ - запрос отправки данных для обновления
```csharp
this.defaultClientService.PatchAsync(
    "api/payments/{id}".ToTemplateUri(command.PaymentId),
    command.Amount,
    cancellationToken);
```
8. _DeleteAsync_ - запрос удаления данных
```csharp
this.defaultClientService.DeleteAsync(
    "api/payments/{id}".ToTemplateUri(command.PaymentId),
    cancellationToken);
```

