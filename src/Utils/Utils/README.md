# Gems.Utils

Эта библиотека .NET предоставляет различные расширения для работы с Linq. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка)
* [Создание хэша MD5](#cоздание-хэша-md5)
* [Приведение даты и времени к UTC](#приведение-даты-и-времени-к-UTC)
* [Зашифровать в RSA 512 и расшифровать](#зашифровать-в-rsa-512-и-расшифровать)

# Установка
- Установите nuget пакет **Gems.Utils** через менеджер пакетов.

# Создание хэша MD5
string CreateMd5(string input)
```csharp
    HashUtil.CreateMd5("some string");
```

# Приведение даты и времени к UTC

Воспользуйтесь атрибутом _UnspecifiedToUtcDateTimeAttribute_ и примените его к свойствам модели
```csharp
    public class ExampleModel
    {
        // another props
        
        [UnspecifiedToUtcDateTime]
        public DateTime CreatedDt { get; set; }
    }
```

Примените приведение ко всем элементам коллекции с моделями
```csharp
    
    exampleModels.ForEach(DateTimeUtils.SetUnspecifiedToUtcDateTime);
```
# Зашифровать в RSA 512 и расшифровать
Сгенерировать закрытый ключ RSA
```csharp    
openssl genrsa -out private_key.pem 512   
```
Экспортировать открытый ключ RSA из закрытого ключа
```csharp    
openssl rsa -in private_key.pem -outform PEM -pubout -out public_key.pem   
```
Пример как зашифровать и расшифровать строку:
```csharp    
var data = "testing";
// Зашифровать строку
var dataBytes = Encoding.UTF8.GetBytes(data);
var base64Encrypted = CryptoUtils.EncryptRsa512(publicKey, dataBytes):
// Расшифровать строку в base64
dataBytes = CryptoUtils.DecryptRsa512(privateKey, base64Encrypted);
data = Encoding.UTF8.GetString(dataBytes);     
```
Перегруженные методы:
```csharp    
var data = "testing";
// Зашифровать строку
var base64Encrypted = CryptoUtils.ConvertToBase64EncryptedWithRsa512(publicKey, data):
// Расшифровать строку
data = CryptoUtils.ConvertFromBase64EncryptedWithRsa512(privateKey, base64Encrypted);     
```