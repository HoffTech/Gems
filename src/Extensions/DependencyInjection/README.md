# Gems.Extensions.DependencyInjection

Эта библиотека .NET предоставляет различные расширения для работы с DI. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка)
* [Расширения для IServiceCollection](#расширения-для-iservicecollection)

# Установка
- Установите nuget пакет **Gems.Extensions.DependencyInjection** через менеджер пакетов.

# Расширения для IServiceCollection

Находит дескриптор указанного типа и при его наличии удаляет его
static void RemoveServiceDescriptor(this IServiceCollection services, Type type)
```csharp
    services.RemoveServiceDescriptor(typeof(SomeType));
```