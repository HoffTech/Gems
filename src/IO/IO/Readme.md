# Gems.IO
Вспомогательная библиотека упрощающая чтение/запись данных с внешними средами

# Содержание
* [Установка](#установка)
* [Описание](#описание)
* [Примеры](#примеры)

# Установка
Установите nuget пакет Gems.IO через менеджер пакетов

# Описание
**Расширение для выгрузки коллекции `IEnumerable<T>` в Excel файл, для работы с данным функционалом необходимо**:
- В классе, описывающем экземпляр коллекции, для свойств выводимых в файл указать атрибут `ColumnHeaderNameAttribute`, в атрибут передать в качестве параметра заголовок колонки, который будет отображаться в файле, например так:
```csharp
public class SaveToFileCollectionElement
{
    /// <summary>
    /// Перечень активных ячеек бизнес-юнита, которые не прикреплены к зоне.
    /// </summary>
    [ColumnHeaderName("Перечень активных ячеек бизнес-юнита, которые не прикреплены к зоне.")]
    public string Wms { get; set; }

    /// <summary>
    /// Перечень зон, по которым отсутствуют товарные группы.
    /// </summary>
    [ColumnHeaderName("Перечень зон, по которым отсутствуют товарные группы.")]
    public string Zone { get; set; }

    /// <summary>
    /// Перечень зон, по которым добавлены склады и ячейки из списка исключений.
    /// </summary>
    [ColumnHeaderName("Перечень зон, по которым добавлены склады и ячейки из списка исключений.")]
    public string ZoneWithExcludedWms { get; set; }

    /// <summary>
    /// Перечень зон, по которым добавлены склады и ячейки из списка исключений.
    /// </summary>
    [ColumnHeaderName("Перечень зон, по которым добавлены ТГ/НГ из списка исключений.")]
    public string ZoneWithExcludedProdCat { get; set; }
}
```
- Вызвать на коллекции метод расширения `SaveToFile`, в качестве параметра передать полное имя файла, реузльтат метода - признак успешности записи.

**Сервис для работы с сетевыми папками общего доступа через SMB протокол.**

- Добавьте конфигурацию настроек подключения к хранилищу в файле appsettings.json:
```csharp
  "SmbStorageOptions": {
    "ShareName": "FileShareTest", // наименование папки общего доступа
    "ServerName": "127.0.0.1", // наименование сервера - хост или IP адрес
    "Credentials": {
      "DomainName": "127.0.0.1", // наименование домена
      "UserName": "", // логин
      "Password": "" // пароль
    }
  }
```
- Зарегистрируйте клиент и опции
```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSmbService(this.Configuration);
        }
```
- Инъектируйте через DI
```csharp

        private readonly SmbStorageService smbStorageService;

        public FileStorageService(SmbStorageClient smbStorageService)
        {
            this.smbStorageService = smbStorageService;
        }
````

# Примеры:
- Удаление файла если существует:
```csharp
  public void DeleteDocumentIfExists(string path)
  {
        this.smbStorageService.DeleteFileIfExists(path);
  }
```
- Чтение файла:
```csharp
  public Task<Stream> GetDocumentAsync(string path)
  {
        return this.smbStorageService.ReadFileAsStreamAsync(path);
  }
````
- Запись файла:
```csharp
  public Task WriteDocumentAsync(string path)
  {
        return this.smbStorageService.WriteFileAsync(path);
  }
````

- Создание директории если не существует:
```csharp
  public void CreateDirectoryIfNotExistsAsync(string path)
  {
        return this.smbStorageService.CreateDirectoryIfNotExists(path);
  }
````