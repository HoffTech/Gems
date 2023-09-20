# Управление версиями gems библиотек 

Версии gems библиотек назначаются автоматически. Изменение версий в файле csproj игнорируются. Поэтому их необходимо указывать, как 0.0.0.0 для того, чтобы не запутать разработчиков, создавая видимость того, что указание версий имеет значение.   
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyVersion>0.0.0.0</AssemblyVersion>
    <FileVersion>0.0.0.0</FileVersion>
    <Version>0.0.0</Version>
  </PropertyGroup>
</Project>
```

Управление версиями gems библиотек осущевствляется за счет комментариев в коммитах. В зависимости от того, какая подстрока (#major, #minor или #patch) содержится в комментарии, производится повышение версии тех библиотек, которые были изменены после последней сборки.
* Подстрока #major - поднять мажорные версии у модифицированных библиотек. Пример: 5.999.999 -> 6.0.0
* Подстрока #minor - поднять минорные версии у модифицированных библиотек. Пример: 5.999.999 -> 5.1000.999
* Подстрока #patch - поднять патч версии у модифицированных библиотек. Данное значение является по умолчанию или обязательным, если коммит является мержом. Пример: 5.999.999 -> 5.999.1000


Для лучшего пониманию приведу пример, как изменятся версии библиотек при изменениия проекта Gems.Utils.csproj.
На данный проект, ссылаются другие проекты. Затем на эти другие проекты ссылаются еще другие проекты. Цепочки ссылающихся проектов будут выглядить так:
```csharp
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Caching.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Data.csproj -> Gems.Data.csproj -> Gems.Data.Npgsql.csproj -> Gems.Patterns.Outbox.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Data.csproj -> Gems.Data.csproj -> Gems.Data.SqlServer.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Data.csproj -> Gems.Data.csproj -> Gems.Patterns.SyncTables.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Data.csproj -> Gems.Data.Npgsql.csproj -> Gems.Patterns.Outbox.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Http.csproj -> Gems.Http.csproj
Gems.Utils.csproj -> Gems.Metrics.csproj -> Gems.Metrics.Prometheus.csproj
Gems.Utils.csproj -> Gems.Patterns.SyncTables.csproj
```
В следствии чего все проекты, которые будут считаться изменные будут такие:
```csharp
Gems.Utils.csproj
Gems.Metrics.csproj
Gems.Metrics.Prometheus.csproj
Gems.Caching.csproj
Gems.Metrics.Data.csproj
Gems.Metrics.Http.csproj
Gems.Data.csproj
Gems.Data.Npgsql.csproj~~~~
Gems.Data.SqlServer.csproj
Gems.Http.csproj
Gems.Patterns.Outbox.csproj
Gems.Patterns.SyncTables.csproj
```
Допустим версии библиотек были такие:
```csharp
Gems.Utils -> 5.2.3
Gems.Metrics -> 5.3.4
Gems.Metrics.Prometheus -> 5.7.2
Gems.Caching -> 5.1.0
Gems.Metrics.Data -> 5.1.1
Gems.Metrics.Http -> 5.1.7
Gems.Data -> 5.8.1
Gems.Data.Npgsql -> 5.8.2
Gems.Data.SqlServer -> 5.9.1
Gems.Http -> 5.4.4
Gems.Patterns.Outbox -> 5.9.8
Gems.Patterns.SyncTables -> 5.0.0
```
Если в комментарии коммита будет присутствовать #major, то версии библиотек изменятся так:
```csharp
Gems.Utils -> 6.0.0
Gems.Metrics -> 6.0.0
Gems.Metrics.Prometheus -> 6.0.0
Gems.Caching -> 6.0.0
Gems.Metrics.Data -> 6.0.0
Gems.Metrics.Http -> 6.0.0
Gems.Data -> 6.0.0
Gems.Data.Npgsql -> 6.0.0
Gems.Data.SqlServer -> 6.0.0
Gems.Http -> 6.0.0
Gems.Patterns.Outbox -> 6.0.0
Gems.Patterns.SyncTables -> 6.0.0
```
Если в комментарии коммита будет присутствовать #minor, то версии библиотек изменятся так:
```csharp
Gems.Utils -> 5.3.0
Gems.Metrics -> 5.4.0
Gems.Metrics.Prometheus -> 5.8.0
Gems.Caching -> 5.2.0
Gems.Metrics.Data -> 5.2.0
Gems.Metrics.Http -> 5.2.0
Gems.Data -> 5.9.0
Gems.Data.Npgsql -> 5.9.0
Gems.Data.SqlServer -> 5.10.0
Gems.Http -> 5.5.0
Gems.Patterns.Outbox -> 5.10.0
Gems.Patterns.SyncTables -> 5.1.0
```
Если в комментарии коммита будет присутствовать #patch, то версии библиотек изменятся так:
```csharp
Gems.Utils -> 5.2.4
Gems.Metrics -> 5.3.5
Gems.Metrics.Prometheus -> 5.7.3
Gems.Caching -> 5.1.1
Gems.Metrics.Data -> 5.1.2
Gems.Metrics.Http -> 5.1.8
Gems.Data -> 5.8.2
Gems.Data.Npgsql -> 5.8.3
Gems.Data.SqlServer -> 5.9.2
Gems.Http -> 5.4.5
Gems.Patterns.Outbox -> 5.9.9
Gems.Patterns.SyncTables -> 5.0.1
```
Если в комментарии коммита указать в конце - [skip ci], то cicd пайплайн не запустится и сборка нугет пакетов не будет произведена.

Примечание: если последний коммит, который был опубликован будет являться мерж коммитов, то строки [skip ci], #major, #minor, #patch не будут взяты из предыдущих коммитов.   
