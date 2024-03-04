namespace Gems.TestInfrastructure.Utils.Database
{
    public class IndexMetadata
    {
        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }

        public string TableName { get; set; }

        public string IndexName { get; set; }

        public string TypeDesc { get; set; }

        public List<IndexColumnMetadata> Columns { get; set; }

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
                    return this.IndexName == name;
                }
                else if (parts.Length == 2)
                {
                    return
                        this.IndexName == parts[1] &&
                        this.TableName == parts[0];
                }
                else if (parts.Length == 3)
                {
                    return
                        this.IndexName == parts[2] &&
                        this.TableName == parts[1] &&
                        this.TableSchema == parts[0];
                }
                else
                {
                    return false;
                }
            }
            else if (obj is IndexMetadata index)
            {
                return
                    this.TableCatalog == index.TableCatalog &&
                    this.TableSchema == index.TableSchema &&
                    this.TableName == index.TableName &&
                    this.IndexName == index.IndexName &&
                    this.TypeDesc == index.TypeDesc;
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
                (this.IndexName?.GetHashCode() ?? 0) ^
                (this.TypeDesc?.GetHashCode() ?? 0);
        }
    }
}
