// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database
{
    public class ColumnMetadata
    {
        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int? OrdinalPosition { get; set; }

        public string ColumnDefault { get; set; }

        public string IsNullable { get; set; }

        public string DataType { get; set; }

        public int? CharacterMaximumLength { get; set; }

        public int? CharacterOctetLength { get; set; }

        public int? NumericPrecision { get; set; }

        public int? NumericPrecisionRadix { get; set; }

        public int? NumericScale { get; set; }

        public int? DatetimePrecision { get; set; }

        public string CharacterSetCatalog { get; set; }

        public string CharacterSetSchema { get; set; }

        public string CharacterSetName { get; set; }

        public string CollationCatalog { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is string name)
            {
                var parts = name.Split('.');
                if (parts.Length == 1)
                {
                    return this.ColumnName == name;
                }
                else if (parts.Length == 2)
                {
                    return
                        this.ColumnName == parts[1] &&
                        this.TableName == parts[0];
                }
                else if (parts.Length == 3)
                {
                    return
                        this.ColumnName == parts[2] &&
                        this.TableName == parts[1] &&
                        this.TableSchema == parts[0];
                }
                else
                {
                    return false;
                }
            }
            else if (obj is ColumnMetadata column)
            {
                return
                    this.TableCatalog == column.TableCatalog &&
                    this.TableSchema == column.TableSchema &&
                    this.TableName == column.TableName &&
                    this.ColumnName == column.ColumnName &&
                    this.OrdinalPosition == column.OrdinalPosition &&
                    this.ColumnDefault == column.ColumnDefault &&
                    this.IsNullable == column.IsNullable &&
                    this.DataType == column.DataType &&
                    this.CharacterMaximumLength == column.CharacterMaximumLength &&
                    this.CharacterOctetLength == column.CharacterOctetLength &&
                    this.NumericPrecision == column.NumericPrecision &&
                    this.NumericPrecisionRadix == column.NumericPrecisionRadix &&
                    this.NumericScale == column.NumericScale &&
                    this.DatetimePrecision == column.DatetimePrecision &&
                    this.CharacterSetCatalog == column.CharacterSetCatalog &&
                    this.CharacterSetSchema == column.CharacterSetSchema &&
                    this.CharacterSetName == column.CharacterSetName &&
                    this.CollationCatalog == column.CollationCatalog;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
                (this.TableCatalog?.GetHashCode() ?? 0) ^
                (this.TableSchema?.GetHashCode() ?? 0) ^
                (this.TableName?.GetHashCode() ?? 0) ^
                (this.ColumnName?.GetHashCode() ?? 0) ^
                (this.OrdinalPosition?.GetHashCode() ?? 0) ^
                (this.ColumnDefault?.GetHashCode() ?? 0) ^
                (this.IsNullable?.GetHashCode() ?? 0) ^
                (this.DataType?.GetHashCode() ?? 0) ^
                (this.CharacterMaximumLength?.GetHashCode() ?? 0) ^
                (this.CharacterOctetLength?.GetHashCode() ?? 0) ^
                (this.NumericPrecision?.GetHashCode() ?? 0) ^
                (this.NumericPrecisionRadix?.GetHashCode() ?? 0) ^
                (this.NumericScale?.GetHashCode() ?? 0) ^
                (this.DatetimePrecision?.GetHashCode() ?? 0) ^
                (this.CharacterSetCatalog?.GetHashCode() ?? 0) ^
                (this.CharacterSetSchema?.GetHashCode() ?? 0) ^
                (this.CharacterSetName?.GetHashCode() ?? 0) ^
                (this.CollationCatalog?.GetHashCode() ?? 0);
        }
    }
}
