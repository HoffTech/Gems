using System.Data.Common;
using System.Text;

using Dapper;

namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    internal class FakeDataBuilder<TEntity> :
        IFakeDataBuilder<TEntity>,
        IFakeDataTableBuilder<TEntity>
        where TEntity : class
    {
        private readonly DbConnection connection;
        private readonly Func<TEntity> generator;
        private string tableName;
        private int minCount = 10;
        private int maxCount = 10;
        private string sqlStatement;

        internal FakeDataBuilder(
            DbConnection connection,
            Func<TEntity> generator)
        {
            this.generator = generator;
            this.connection = connection;
        }

        public IFakeDataBuilder<TEntity> ForTable(string tableName)
        {
            this.tableName = tableName;
            this.BuildSqlStatement();
            return this;
        }

        public IFakeDataBuilder<TEntity> WithCount(int count)
        {
            this.minCount = count;
            this.maxCount = count;
            return this;
        }

        public IFakeDataBuilder<TEntity> WithCount(int minCount, int maxCount)
        {
            if (minCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minCount));
            }

            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            if (maxCount < minCount)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            this.minCount = minCount;
            this.maxCount = maxCount;
            return this;
        }

        public async Task BuildAsync(CancellationToken cancellationToken = default)
        {
            var random = new Random();
            var recordCount = random.Next(this.minCount, this.maxCount + 1);
            var connectionType = this.connection.GetType();
            for (var i = 0; i < recordCount; i++)
            {
                var entity = this.generator();
                if (entity != null)
                {
                    await this.connection.ExecuteAsync(
                        this.sqlStatement,
                        entity);
                }
            }
        }

        private void BuildSqlStatement()
        {
            var connectionType = this.connection.GetType();
            var paramPrefix = connectionType.Name.Equals("NpgsqlConnection") ? ":" : "@";
            var propNames = typeof(TEntity).GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance)
                .Select(p => p.Name);
            var statement = new StringBuilder();
            statement
                .Append("INSERT INTO ")
                .Append(this.tableName)
                .Append(" (")
                .Append(string.Join(", ", propNames))
                .Append(") VALUES (")
                .Append(string.Join(", ", propNames.Select(p => paramPrefix + p)))
                .Append(");");
            this.sqlStatement = statement.ToString();
        }
    }
}
