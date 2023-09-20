// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Gems.Data.SqlServer;

public class MsSqlUnitOfWorkOptionsList : List<MsSqlUnitOfWorkOptions>
{
    /// <summary>
    /// Name in appsettings.json.
    /// </summary>
    public const string Name = "MsSqlUnitOfWorks";
}
