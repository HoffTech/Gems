namespace Gems.TestInfrastructure.Utils.Database
{
    public class IndexColumnMetadata
    {
        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string IndexName { get; set; }

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
                        this.IndexName == parts[0];
                }
                else if (parts.Length == 3)
                {
                    return
                        this.ColumnName == parts[2] &&
                        this.IndexName == parts[1] &&
                        this.TableName == parts[0];
                }
                else if (parts.Length == 4)
                {
                    return
                        this.ColumnName == parts[3] &&
                        this.IndexName == parts[2] &&
                        this.TableName == parts[1] &&
                        this.TableSchema == parts[0];
                }
                else
                {
                    return false;
                }
            }
            else if (obj is IndexColumnMetadata indexColumn)
            {
                return
                    this.TableCatalog == indexColumn.TableCatalog &&
                    this.TableSchema == indexColumn.TableSchema &&
                    this.TableName == indexColumn.TableName &&
                    this.ColumnName == indexColumn.ColumnName &&
                    this.IndexName == indexColumn.IndexName;
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
                (this.IndexName?.GetHashCode() ?? 0);
        }
    }
}
