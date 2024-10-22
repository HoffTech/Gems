// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database.Import
{
    public interface IImportRow
    {
        string[] GetColumns();

        object GetValue(int i);
    }
}
