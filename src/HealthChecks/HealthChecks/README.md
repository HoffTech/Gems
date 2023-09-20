# Gems.HealthChecks

Библиотека регистрирует две конечные точки для проверки доступности сервиса

* /liveness
* /readiness 

Пример настройки проб:

	services.AddDefaultHealthChecks(op => op.DefaultServiceIsReady = false);

    ...

    app.UseEndpoints(endpoints =>
    {
        ...
        endpoints.MapDefaultHealthChecks();
    });

По умолчанию пробы возвращают положительный результат. 
Для управления статусом liveness получите через DI сервис ILivenessProbe и выставите свойство ServiceIsAlive.

    var probe = sp.GetRequiredService<ILivenessProbe>();
    probe.ServiceIsAlive = false;


Для управления статусом readiness получите через DI сервис IReadinessProbe и выставите свойство ServiceIsReady.


    var probe = sp.GetRequiredService<IReadinessProbe>();
    probe.ServiceIsReady = false;
    ... // длительная блокирующая операция
    probe.ServiceIsReady = true;

Вы можете добавить собственные проверки, кроме тех, что устанавливаются по-умолчанию. Пример:

    services
        .AddDefaultHealthChecks()
        .AddCheck<MyLivenessCheck>()
        .AddCheck<MyReadinessCheck>("MyReadinessCheckName", tags: new[] { "ready" });

Настройки в valuses.yaml, для использования проб:

    Probes:
    liveness:
        path: /liveness
        port: http
    readiness:
        path: /readiness
        port: http
