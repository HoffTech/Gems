﻿// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    public interface IFakeDataTableBuilder<TRecord> where TRecord : class
    {
        IFakeDataBuilder<TRecord> ForTable(string tableName);
    }
}
