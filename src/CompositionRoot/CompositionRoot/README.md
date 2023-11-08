# Gems.Context

Эта библиотека .NET содержит методы конфигурации gems библиотек.

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

### Внешние библиотеки
Данная библиотека подключает такие внешние библиотеки:
- Autofac
- Autofac.Extensions.DependencyInjection
- AutoMapper
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets
- NLog
- NLog.Web.AspNetCore
- prometheus-net.AspNetCore
### Gems библиотеки
Данная библиотека подключает такие gems библиотеки:
- Gems.BusinessRules
- Gems.Caching
- Gems.Data.MySql
- Gems.Data.Npgsql
- Gems.Data.SqlServer
- Gems.HealthChecks
- Gems.Http.csproj
- Gems.Jobs.csproj
- Gems.Jobs.Quartz
- Gems.MessageBrokers.Kafka
- Gems.Metrics.Prometheus
- Gems.Swagger
Необходимо помнить, что вместе с указанными gems библиотеками, подключаются все зависимые gems и внешние библиотеки. Тем самым для приложения подключаютс почти все необходимые библиотеки.

### Конфигурация gems библиотек в startup
Данная библиотека предоставляет такие методы для конфигурирования gems и внешних библиотек:
- AddControllersWithMediatR
- AddPrometheus
- AddHealthChecks
- AddSwagger
- AddAutoMapper
- AddMediatR
- AddPipelines
- AddValidation
- AddJobs (Quartz)
- AddUnitOfWorks
- AddHttpServices
- AddDistributedCache
- RegisterServices
- AddSecureLogging
- AddProducers
- AddConsumers
### Когда можно использовать библиотеку Gems.CompositionRoot?
Необходимо убедиться, что в вашем проекте используются все библиотеки, указанные в разделах "Внешние библиотеки" и "Gems библиотеки". Если используются другие аналогичные библиотеки (Serilog, Hangfire и тп.), то библиотеку Gems.CompositionRoot нельзя использовать. Если используется все указанные библиотеки, плюс еще дополнительные, то библиотеку Gems.CompositionRoot можно использовать, но дополнительные библиотеки придется конфигурировать самостоятельно.

### Настройка CompositionRoot
- Удалите из проекта все библиотеки, указанные в разделах "Внешние библиотеки" и "Gems библиотеки"
- Установите последнюю версию библиотеки Gems.CompositionRoot
- Startup приведите к следующему виду:
```csharp
public class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCompositionRoot<Startup>(this.configuration, options =>
        {
            // ... Переопределите конфигурацию по умолчанию, используя методы из раздела "Конфигурация gems библиотек в startup"
        });
        // ... Конфигурация дополнительных библиотек
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.ConfigureAutofacCompositionRoot<Startup>();
        // ... Конфигурация дополнительных библиотек
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwaggerApi(this.configuration);
        app.UseEndpointLogging();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
            endpoints.MapDefaultHealthChecks();
        });
    }
}
```
