# Gems.Text.Json  

Вспомогательная библиотека сериализации/десериализации объектов.

# Содержание
* Описание
* Установка
* Примеры
* Конвертеры

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
# Конвертеры
Есть такие пользовательские конвертеры:  DoubleFormatConverter, FloatFormatConverter, EmptyStringToNullConverter и UnspecifiedDateTimeConverter.
Если нужно сериализовать, то опишем класс так:
```csharp
    public class ValuesHolder
    {
        [DoubleRoundConverter(DecimalDigitsLength = 3)]
        public double Value1 { get; set; } = 123.4567;

        [FloatRoundConverter(DecimalDigitsLength = 3)]
        public float Value2 { get; set; } = 123.4567F;

        [EmptyStringToNullConverter]
        public string Value3 { get; set; } = string.Empty;

        [UnspecifiedDateTimeConverter(SerializerTimeZone = "Europe/Moscow")]
        public DateTime Value4 { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(+3), DateTimeKind.Unspecified);
    }    
```
Сериализуем:
```csharp
    var data = new ValuesHolder();
    var serializedData = data.Serialize(camelCase: true);
```
Json получится такой:
```json
{
	"value1":123.457,
	"value2":123.457,
	"value3":null,
	"value4":"2024-05-21T03:18:54+03:00"
}
```
Если нужно десериализовать, то опишем класс так:
```csharp
    public class ValuesHolder
    {
        public double Value1 { get; set; }

        public float Value2 { get; set; }
        
        [EmptyStringToNullConverter]
        public string Value3 { get; set; }

        [UnspecifiedDateTimeConverter(DeserializerTimeZone = "Europe/Moscow")]
        public DateTime Value4 { get; set; }
    }
```
Десериализуем такой Json:
```json
{
    "value1":123.4567,
    "value2":123.4567,
	"value3":"",
	"value4":"2024-05-21T03:18:54"
}
```
После десериализации данные будут такие:
```csharp
    var deserializedData = data.Deserialize<ValuesHolder>();
    Console.WriteLine(deserializedData.Value1);
    Console.WriteLine(deserializedData.Value2);
    Console.WriteLine(deserializedData.Value3);
    Console.WriteLine(deserializedData.Value4ю);
    // Вывод на консоле:
    123.4567
    123.4567

    2024-05-21T03:18:54+03:00
```