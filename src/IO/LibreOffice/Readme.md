# Gems.IO.LibreOffice
Библиотека, предоставляющая Api работы с LibreOffice

# Содержание
* [Установка](#установка)
* [Описание](#описание)
* [Примеры](#примеры)

# Установка
Установите nuget пакет `Gems.IO.LibreOffice` через менеджер пакетов

# Описание
Библиотека представляет Wrapper Api для работы с LibreOffice. LibreOffice работает в многопоточном режиме и для каждой операции создается новый временный пользователь, который впоследствии будет удален.

**Добавьте конфигурацию настроек LibreOffice в файле appsettings.json:**
- Пример конфигурации для Linux
```json
  "LibreOfficeOptions": {
    "LibreOfficeExecutablePath": "/usr/bin/soffice", // путь к исполняемому файлу LibreOffice.
    "TempUserPathForArgs": "tmp/user", // путь к папке для генерации временных пользователей LibreOffice для аргументов команды.
    "TempUserDirectPathForDelete": "../tmp/user" // прямой путь к папке для удаления сгенерированных временных пользователей LibreOffice.
    }
  }
```
Для `DockerFile` используйте образ `${CI_REGISTRY}/build-images/core/aspnet:6.0-libreoffice`
В нем размещен скрипт, который можно использовать для локальной проверки конвертации в Linux
```dockerfile
RUN apt-get update -y
RUN apt-get install -y --no-install-recommends \
        default-jre \
		libreoffice \
		libreoffice-writer \
		ure \
		libreoffice-java-common \
		libreoffice-core \
		libreoffice-common \
		fonts-opensymbol \
		hyphen-fr \
		hyphen-de \
		hyphen-en-us \
		hyphen-it \
		hyphen-ru \
		fonts-dejavu \
		fonts-dejavu-core \
		fonts-dejavu-extra \
		fonts-droid-fallback \
		fonts-dustin \
		fonts-f500 \
		fonts-fanwood \
		fonts-freefont-ttf \
		fonts-liberation \
		fonts-lmodern \
		fonts-lyx \
		fonts-sil-gentium \
		fonts-texgyre \
		fonts-tlwg-purisa 

RUN apt-get -y -q remove libreoffice-gnome && \
	apt -y autoremove

RUN rm -rf /var/lib/apt/lists/*

RUN adduser --home=/opt/libreoffice --disabled-password --gecos "" --shell=/bin/bash libreoffice
```
- Пример конфигурации для Windows (портабельный LibreOffice)
```json
   // Пример конфигурации для Linux
  "LibreOfficeOptions": {
    "LibreOfficeExecutablePath": "..\\LibreOfficePortable\\App\\libreoffice\\program\\soffice.exe", // путь к исполняемому файлу LibreOffice.
    "TempUserPathForArgs": "$ORIGIN/../../../Data/settings", // путь к папке для генерации временных пользователей LibreOffice для аргументов команды.
    "TempUserDirectPathForDelete": "..\\LibreOfficePortable\\Data\\settings", // прямой путь к папке для удаления сгенерированных временных пользователей LibreOffice.
    "MaxConcurrentLibreOfficeInstances": 50 // максимальное количество параллельно запущенных экземпляров LibreOffice.
    }
  }
```
При этом необходимо уточнить файл bootstrap.ini, чтобы конфигурация смапилась
```ini
[Bootstrap]
InstallMode=<installmode>
ProductKey=LibreOffice 7.4
UserInstallation=$ORIGIN/../../../Data/settings
[ErrorReport]
ErrorReportServer=
```

- Зарегистрируйте клиент и опции
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLibreOffice(this.Configuration);
}
```
- Инъектируйте через DI
```csharp
private readonly ILibreOffice libreOffice;

public FileService(ILibreOffice libreOffice)
{
    this.libreOffice = libreOffice;
}
````

# Примеры:
- Конвертация Docx в Pdf:
```csharp
  public Task GenerateReportAsync(string docxPath, string pdfPath, CancellationToken cancellationToken)
  {
      return this.libreOffice.ConvertToPdfAsync(docxPath, pdfPath, cancellationToken);
  }
```

