// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.Options
{
    public class UpsertVersionFunctionInfo
    {
        public string FunctionName { get; set; }

        public string TableParameterName { get; set; }

        public string RowVersionParameterName { get; set; }
    }
}
