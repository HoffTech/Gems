# Gems.Logging.Security

Библиотека позволяет фильтровать чувстивтельные данные, удаляя или маскируя иx в JSON и XML

## Настройки Gitlab

Для использования данной билиотеки следует добавить nuget пакет Gems.Logging.Security
В файл Startup.cs добавьте следующий код

    services.AddLoggingFilter(builder => 
    {
        builder.Register(new SecureKeyCsvFileSource("securekeys.txt"));
    });

Если требуется получить csv файл через интернет, то

    services.AddLoggingFilter(builder => 
    {
        builder.Register(new SecureKeyCsvHttpSource(new Uri("http://myserver/securekeys.txt")));
    });

Если требуется получить json файл через интернет, то

    services.AddLoggingFilter(builder => 
    {
        builder.Register(new SecureKeyJsonHttpSource(new Uri("http://myserver/securekeys.json")));
    });

Для фильтрации получите через DI экземпляр фильтра

    var filter = sp.GetRequiredService<IPropertyFilter<JToken>>(); // JSON


    var filter = sp.GetRequiredService<IPropertyFilter<XElement>>(); // XML


    var filter = sp.GetRequiredService<IPropertyFilter<ObjectToJsonProjection>>(); // Any object

Фильтрация Json
    
    var result = filter.FilterJson(@"{ ""phone"": ""9165550206"" }");

Фильтрация Xml

    var result = filter.FilterXml("<root><phone>9165550206</phone></root>");

Фильтрация объекта

    var result = filter.FilterObject(new { Phone = "9165550206" });

Для декларации свойств, которые должны фильтроваться используйте FilterAttribute

    internal class SimpleObject
    {
        [Filter]
        public string HideMe { get; set; }
        public string Dummy { get; set; }
        public SimpleObject2 SubObject { get; set; }

        public class SimpleObject2
        {
            [Filter(".(?=.{4})", "*")]
            public string MaskMe { get; set; }
            public string Dummy { get; set; }
        }
    }