// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Newtonsoft.Json.Linq;

namespace Gems.Logging.Security
{
    public class ObjectToJsonProjection
    {
        public ObjectToJsonProjection(object obj, JToken token)
        {
            this.Object = obj;
            this.Token = token;
            this.ObjectType = obj?.GetType() ?? typeof(string);
        }

        public JToken Token { get; }

        public Type ObjectType { get; }

        public object Object { get; }
    }
}
