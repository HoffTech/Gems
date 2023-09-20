# Gems.Text.Json  

Вспомогательная библиотека сериализации/десериализации объектов.

# Содержание
* Описание
* Установка
* Примеры

# Описание
Метод __Serialize()__ выполняет сериализацию объекта в JSON строку. У метода есть два параметра по умолчанию:
1. __IList<JsonConverter> additionalConverters__ - список дополнительных конвертеров.
2. __JavaScriptEncoder encoder__ - объект, который указывает, какие символы кодировщику разрешено не кодировать

Метод __Deserialize<T>()__ выполняет десериализацию JSON строки в указанный объект. У метода есть один параметр по умолчанию:
1. __IList<JsonConverter> additionalConverters__ - список дополнительных конвертеров.

Так же, в библиотеке определен конвертер JSON строки в Dictionary<string, object> - DictionaryStringObjectConverter

# Установка
Установите nuget пакет Gems.Text.Json через менеджер пакетов

# Примеры
Для использования библиотеки добавьте __using Gems.Text.Json__

* Сериализация с помощью метода Serialize():
    ```csharp
    public class TestClass
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }
    }
    
    var testClass = new TestClass { Field1 = "test", Field2 = 1 };
    
    var serializedTestClass = testClass.Serialize();
    
    // Результат: serializedTestClass = "{\"Field1\":\"test\",\"Field2\":1}"
    ```
* Десериализация с помощью метода Deserialize<T>():
    ```csharp
    public class TestClass
    {
        public string Field1 { get; set; }
        public int Field2 { get; set; }
    }
    
    var testClass = new TestClass { Field1 = "test", Field2 = 1 };
    
    var serializedTestClass = testClass.Serialize();
    
    var testClass2 = serializedTestClass.Deserialize<TestClass>();
    
    // Результат:
    // testClass2.Field1 = "test"
    // testClass2.Field2 = 1
    ```