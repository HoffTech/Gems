# Скачивание сертификата для возможности отправки запросов по Https

Для возможности отправки запросов по протоколу `https` необходимо установить опцию загрузки сертификата в значение `true`

Конфигурацию можно осуществить
 1. В `appsettings.json`
    ```json
          "PaymentApi": {
            "Durable": false,                       
            "DurableFromHttpStatus": 499,           
            "Attempts": 0,                          
            "MillisecondsDelay": 0,                 
            "RequestTimeout": 30000,               
            "NeedDownloadCertificate": true,        // Признак загрузки сертификата
            "BaseUrl": "http://localhost:5135/"
          }
    ```
2. В классе наследника `BaseClientService`
```csharp
    public class PaymentService(IOptions<PaymentApiOptions> options, BaseClientServiceHelper helper)
        : BaseClientService<BusinessErrorViewModel>(options, helper)
    {
        protected override bool NeedDownloadCertificate { get; } = true;
        
        // ...
    }
```
