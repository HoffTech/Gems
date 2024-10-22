// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database
{
    public class DatabaseMetadata
    {
        public string DatabaseName { get; set; }

        public string Owner { get; set; }

        public string Encoding { get; set; }

        public List<TableMetadata> Tables { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is string databaseName)
            {
                return this.DatabaseName.Equals(databaseName);
            }
            else if (obj is DatabaseMetadata database)
            {
                return
                    this.DatabaseName == database.DatabaseName &&
                    this.Owner == database.Owner &&
                    this.Encoding == database.Encoding;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
                (this.DatabaseName?.GetHashCode() ?? 0) ^
                (this.Owner?.GetHashCode() ?? 0) ^
                (this.Encoding?.GetHashCode() ?? 0);
        }
    }
}
