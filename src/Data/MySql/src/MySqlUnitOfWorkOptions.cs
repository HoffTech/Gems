// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Data.UnitOfWork;

namespace Gems.Data.MySql;

public class MySqlUnitOfWorkOptions
{
    public string Key { get; set; }

    public UnitOfWorkOptions Options { get; set; }
}
