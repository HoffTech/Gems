// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Logging.Security
{
    public interface IPropertyProxy
    {
        Type RootType { get; }

        string Name { get; }

        object Value { get; }

        void Remove();

        void Update(string value);
    }
}
