namespace Gems.TestInfrastructure.Utils.Database
{
    public class TableMetadata
    {
        public string TableCatalog { get; set; }

        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public string TableType { get; set; }

        public List<ColumnMetadata> Columns { get; set; }

        public List<IndexMetadata> Indexes { get; set; }

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
                    return this.TableName == name;
                }
                else if (parts.Length == 2)
                {
                    return
                        this.TableSchema == parts[0] &&
                        this.TableName == parts[1];
                }
                else
                {
                    return false;
                }
            }
            else if (obj is QualifiedTableName qualifiedName)
            {
                return
                    qualifiedName.TableName.Equals(this.TableName) &&
                    (this.TableName == null || qualifiedName.TableSchema == null || qualifiedName.TableSchema.Equals(this.TableSchema));
            }
            else if (obj is TableMetadata table)
            {
                return
                    this.TableCatalog == table.TableCatalog &&
                    this.TableSchema == table.TableSchema &&
                    this.TableName == table.TableName &&
                    this.TableType == table.TableType;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
                this.TableCatalog.GetHashCode() ^
                this.TableName.GetHashCode() ^
                this.TableSchema.GetHashCode() ^
                this.TableType.GetHashCode();
        }
    }
}
