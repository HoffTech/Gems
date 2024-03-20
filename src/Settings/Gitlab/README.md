# Gems.Settings.Gitlab

Gems.Settings.Gitlab позволяет хранить настройки в Gitlab и обновлять их в фоновом режиме или принудительно post запросом. Также есть возможность получать значения переменных Gitlab по их названию.  

## Настройки Gitlab

Для использования данной библиотеки следует найтроить ваш проект в Gitlab. Рассмотрим пример настроки. 
Перейдите в секцию Settings-->Access tokens и создайте токен с доступом read_api. 
Скопируйте токен. Перейдите в секцию Settings-->CI/CD-->Variables. 
Заведите в ней 3 переменные:

* GITLAB_CONFIGURATION_URL = <<gitlab_root_url>>
* GITLAB_CONFIGURATION_TOKEN = <<Созданный ранее токен>>
* GITLAB_CONFIGURATION_PROJECTID = <<Ид вашего проекта>>

Добавьте в проект пакет Gems.Settings.Gitlab

В файле values.yaml укажите перменные огружения:

    EnvVars:
        GITLAB_CONFIGURATION_URL: "__GITLAB_CONFIGURATION_URL__"
        GITLAB_CONFIGURATION_TOKEN: "__GITLAB_CONFIGURATION_TOKEN__"
        GITLAB_CONFIGURATION_PROJECTID: "__GITLAB_CONFIGURATION_PROJECTID__"

В файле Program.cs укажите использование источника конфиграции Gitlab

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder.AddGitlabConfiguration();
            }) 
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

В файле Startup.cs добавьте автоматическое обновление конфигурации:

    services.AddGitlabConfigurationUpdater();

## Настройки способов обновления

Если хотите настроить способы обновления, файле Startup.cs в строку включения автоматического обновления добавьте конфигурацию:
```
    services.AddGitlabConfigurationUpdater(configuration);
```

В файле appsettings заведите секцию `GitlabSettings` и укажите в ней 2 переменные - `EnableBackgroundUpdater` и `EnableEndpointUpdater`.
`EnableBackgroundUpdater` = true запустит фоновое обновление настроек раз в 5 минут. `EnableEndpointUpdater` = true добавит контроллер запроса POST `gitlabsettings/update`.
Значение по умолчанию для `EnableBackgroundUpdater` = true, для `EnableEndpointUpdater` = true.
Пример:

```
  "GitlabSettings": {
    "EnableBackgroundUpdater": true,
    "EnableEndpointUpdater": true
  }
```

## Получение значения настройки в коде
Если необходимо получить значение Gitlab переменной в коде, добавьте GitlabConfigurationValuesProvider в конструктор. 
У него есть 2 метода:
```
public async Task<T> GetGitlabVariableValueByName<T>(string variableName)
public async Task<string> GetGitlabVariableValueByName(string variableName)
```
Первый пытается десериализовать полученное из Gitlab значение в параметризованный класс. Второй возвращает значение переменной в виде строки.

## Система приоретизации переменных
В конфигурацию попадут все переменные gitlab, у которых:
1) есть префикс среды. Например, DEV_SettingName попадёт в конфигурацию в dev среде, но будет проигнорирована на stage и prod;
2) нет префикса среды и указан Environments для переменной в Gitlab. Аналогично предыдущему пункту, в конфигурацию попадут только те переменные, у которых Environments совпадает с текущей;
3) нет префикса среды и не указан Environments для переменной в Gitlab;

В случае конфликтов приоритет будет отдан префиксу в названии (п.1). Если конфликт между переменными как в п.2 и п.3, то приоритет будет отдан значению из п.2.

Например, в Gitlab есть 4 переменных:
```
|    Key   | Value | Environments  |
|----------|-------|---------------|
|DEV_Name  |   1   |       *       |
|  Name    |   2   |       Dev     |
|  Name    |   3   |       Stage   |
|  Name    |   4   |       *       |
```
В таком случае для `"ASPNETCORE_ENVIRONMENT": "Development"` значение переменной `Name` будет равно 1,
для `"ASPNETCORE_ENVIRONMENT": "Staging"` - 3, для `"ASPNETCORE_ENVIRONMENT": "Production"` - 4.