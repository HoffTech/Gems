# Gems.Settings.Gitlab

Gems.Settings.Gitlab позволяет хранить настройки в Gitlab и периолически их обновлять. 

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
