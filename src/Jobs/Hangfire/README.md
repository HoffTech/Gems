# Gems.Jobs.Hangfire

Содержит вспомогательные классы для сервисов-планировщиков на основе Hangfire

Библиотека предназначена для следующих сред выполнения (и новее):

.NET 6.0

# Содержание

* [Установка](#установка)
* [Регистрация задания](#регистрация-задания)

# Установка
- Установите nuget пакет Gems.Jobs.Quartz через менеджер пакетов
- Добавьте следующие строки в appsettings
```json
"Workers": {
  "ImportCommentsFromDax": "0/5 * * * *" // триггер, где ключ - наименование задания, значение - крон выполнения
},
"HangfireWorkerCount": 6,
```
- Добавьте регистрацию Hangfire в конфигурацию сервисов в классе Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services)
{
    //...
    services.AddHangfire(
        this.Configuration.GetConnectionString("<connection_string>"),
        this.Configuration.GetValue<int>("HangfireWorkerCount"));
    //...
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //...
    app.AddHangfireJobsFromAssemblyContaining<Startup>(
        jobName => this.Configuration.GetValue<string>($"Workers:{jobName}"));
    //...
}
```

# Регистрация задания
Чтобы зарегистрировать конкурентную задачу необходимо к классу **Handler** добавить атрибут **JobHandler** и в конструктор для параметра **isConcurrent** передать значение **true**

```csharp
    [JobHandler("ImportCommentsFromDax")]
    public class SignDocumentsCommandHandler : IRequestHandler<ImportCommentsFromDaxCommand>
    {
        //...
    ]
```