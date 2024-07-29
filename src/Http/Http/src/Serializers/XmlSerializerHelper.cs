// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Gems.Http.Serializers;

public static class XmlSerializerHelper
{
    public static TResponse DeserializeObjectFromXml<TResponse>(string xml)
    {
        var serializer = new XmlSerializer(typeof(TResponse));
        using TextReader reader = new StringReader(xml);
        return (TResponse)serializer.Deserialize(reader);
    }

    public static string SerializeObjectToXml(object obj)
    {
        var serializer = new XmlSerializer(obj.GetType());
        using var stream = new MemoryStream();
        var xmlNamespaces = new XmlSerializerNamespaces();
        xmlNamespaces.Add(string.Empty, string.Empty);
        serializer.Serialize(stream, obj, xmlNamespaces);
        var content = Encoding.UTF8.GetString(stream.ToArray());
        content = new Regex("^<\\?xml version=\"1.0\"\\?>").Replace(
            content,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        return content;
    }
}
