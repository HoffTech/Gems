# Установка Histogram метрики

**Для того чтобы использовать метрику типа _Histogram_ необходимо:**
1. В классе `Startup.cs` подключите маппинг метрик
    ```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }
    ```
2. Настройте конфигурацию бакетов для гистограммы и укажите наименование метрики
```json
  "PrometheusMetrics": {
    "Configuration": {
      "HistogramConfiguration": {
        "persons_age_histogram": {
          // Шкала гистограммы по возрасту персон
          "Buckets": [ 18, 25, 30, 40, 50, 60 ] 
        }
      }
    }
  }
```

3. Создайте перечисление `CreatePersonMetricType` для хранения метрики
    ```csharp
    public enum CreatePersonMetricType
    {
        [Metric(
            Name = "persons_age_histogram", // Наименование метрики такое же, как в конфигурации Гистограммы в п.2
            Description = "Гистограмма по возрасту персон")]
        PersonsAgeHistogram
    }
}
    ```
4. Вызовите метод установки _Histogram_ метрики
    ```csharp
    public async Task<PersonDto> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
    {
        // ...

        // сбор значений для Гистограммы по возрасту персон
        await metricsService
            .Histogram(CreatePersonMetricType.PersonsAgeHistogram, command.Age)
            .ConfigureAwait(false);
    
        // ...
    }
    ```
5. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics` и зафиксируйте результат записи метрик в миллисекунлах
    ```
    # HELP persons_age_histogram Гистограмма по возрасту персон
    # TYPE persons_age_histogram histogram
    persons_age_histogram_sum 250
    persons_age_histogram_count 8
    persons_age_histogram_bucket{le="18"} 1
    persons_age_histogram_bucket{le="25"} 2
    persons_age_histogram_bucket{le="30"} 5
    persons_age_histogram_bucket{le="40"} 7
    persons_age_histogram_bucket{le="50"} 8
    persons_age_histogram_bucket{le="60"} 8
    persons_age_histogram_bucket{le="+Inf"} 8
    ```

Расшифровка:
- `persons_age_histogram_sum` - сумма всех возрастов персон
- `persons_age_histogram_count` - общее количество персон
- `persons_age_histogram_bucket{le="18"}` - количество персон с возрастом менее или равным `18`
- `persons_age_histogram_bucket{le="25"}` - количество персон с возрастом менее или равным `25`
- `persons_age_histogram_bucket{le="30"}` - количество персон с возрастом менее или равным `30`
- `persons_age_histogram_bucket{le="40"}` - количество персон с возрастом менее или равным `40`
- `persons_age_histogram_bucket{le="50"}` - количество персон с возрастом менее или равным `50`
- `persons_age_histogram_bucket{le="60"}` - количество персон с возрастом менее или равным `60`
- `persons_age_histogram_bucket{le="+Inf"}` - количество персон с возрастом менее или равным `бесконечности`

### Запуск примера
1. Вызовите ендпоинт `api/v1/persons/create` с помощью **Swagger**
2. Перейдите на страницу `http(s)://<your_domain>:<your_port>/metrics`
