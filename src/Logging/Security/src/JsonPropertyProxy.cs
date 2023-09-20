// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Security
{
    internal class JsonPropertyProxy : IPropertyProxy
    {
        public JsonPropertyProxy(JProperty token)
        {
            this.Token = token;
        }

        public JProperty Token { get; set; }

        public string Name => this.Token.Name;

        public object Value => this.Token.Value;

        public Type RootType => null;

        public void Remove()
        {
            this.Token.Remove();
        }

        public void Update(string value)
        {
            this.Token.Value = new JValue(value);
        }
    }

    internal class JsonTypedPropertyProxy : IPropertyProxy
    {
        public JsonTypedPropertyProxy(JProperty token, Type rootType)
        {
            this.Token = token;
            this.RootType = rootType;
        }

        public JProperty Token { get; set; }

        public string Name => this.Token.Name;

        public object Value => this.Token.Value;

        public Type RootType { get; }

        public void Remove()
        {
            this.Token.Remove();
        }

        public void Update(string value)
        {
            this.Token.Value = new JValue(value);
        }
    }

    internal class JsonArrayElementProxy : IPropertyProxy
    {
        public JsonArrayElementProxy(JValue token)
        {
            this.Token = token;
        }

        public JValue Token { get; set; }

        public string Name => (this.Token.Parent!.Parent as JProperty)?.Name ?? string.Empty;

        public object Value => this.Token.Value!;

        public Type RootType => null;

        public void Remove()
        {
            this.Token.Remove();
        }

        public void Update(string value)
        {
            this.Token.Value = value;
        }
    }
}
