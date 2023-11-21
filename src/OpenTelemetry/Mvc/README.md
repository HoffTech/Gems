# OpenTelemetry

## Введение

Библиотека Gems.OpenTelemetry призвана упростить подключение трассировки OpenTelemetry в ASP.NET Core MVC приложениях.


## Подключение пактов Nuget

Подключите к вашему приложению пакет Gems.OpenTelemetry командой 

```
dotnet add package Gems.OpenTelemetry 
dotnet add package Gems.OpenTelemetry.Mvc
```

## Включение трассировки команд

Чтобы добавить трассировку команд добавьте следующую строку, которая добавит в пайплайн MediatR фильтр

```
services.AddPipeline(typeof(TracingBehavior<,>));
```

После этого исполнение команд станет попадать в трассировку

## Настройка атрибутов

Для настройки атрибутов трассировки выполнения команд MediatR добавьте в секцию `Tracing` узел `Command`.

```
"Tracing": {
    . . .
    "Command": {
        "IncludeRequest": true,
        "IncludeResponse": true
    }
}
```

Значение параметров

- `IncludeRequest` - добавить в трассировку текст команды. По умолчанию этот параметр равен `false`.
- `IncludeResponse` - добавить в трассировку текст ответа на команду. По умолчанию этот параметр равен `false`.

Эту настроку разрешено изменять в ходе работы приложения.


