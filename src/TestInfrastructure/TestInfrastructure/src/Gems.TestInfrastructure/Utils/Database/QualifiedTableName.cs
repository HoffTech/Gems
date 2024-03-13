namespace Gems.TestInfrastructure.Utils.Database
{
    public class QualifiedTableName
    {
        public QualifiedTableName(string tableName, string tableSchema = null)
        {
            ArgumentNullException.ThrowIfNull(tableName, nameof(tableName));
            this.TableName = tableName;
            this.TableSchema = tableSchema;
        }

        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public static QualifiedTableName Parse(string nameAndSchema)
        {
            var parts = nameAndSchema.Split('.');
            if (parts.Length == 2)
            {
                return new QualifiedTableName(parts[1], parts[0]);
            }
            else if (parts.Length == 1)
            {
                return new QualifiedTableName(nameAndSchema, null);
            }
            else
            {
                throw new ArgumentException(nameof(nameAndSchema));
            }
        }

        public override string ToString()
        {
            return this.TableSchema == null ? this.TableName : $"{this.TableSchema}.{this.TableSchema}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is QualifiedTableName qualifiedName)
            {
                return
                    qualifiedName.TableName.Equals(this.TableName) &&
                    (this.TableSchema == null || qualifiedName.TableSchema == null || qualifiedName.TableSchema.Equals(this.TableSchema));
            }
            else if (obj is TableMetadata tableSchema)
            {
                return
                    tableSchema.TableName.Equals(this.TableName) &&
                    (this.TableSchema == null || tableSchema.TableSchema == null || tableSchema.TableSchema.Equals(this.TableSchema));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var i = this.TableName.GetHashCode();
            if (this.TableSchema != null)
            {
                i ^= this.TableSchema.GetHashCode();
            }

            return i;
        }
    }
}
