// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text;

namespace Gems.Http;

public class MultipartFormDataItem
{
    public string Key { get; set; }

    public object Value { get; set; }

    public Encoding Encoding { get; set; } = Encoding.UTF8;

    public string MediaType { get; set; } = "text/plain";

    public string FileName { get; set; }
}
