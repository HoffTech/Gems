// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using AutoFixture;

using FluentAssertions;

using Gems.TestInfrastructure.Assertion.Database;
using Gems.TestInfrastructure.Utils.Database;

namespace Gems.TestInfrastructure.UnitTests.Assertion
{
    public class SchemaAssertions
    {
        private readonly Fixture fixture = new Fixture();

        [TestCase("table")]
        [TestCase("schema.table")]
        public void TableMetadataCollectionContain(string name)
        {
            var list = this.fixture.Create<List<TableMetadata>>();
            list.Add(this.fixture
                .Build<TableMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .Contain(name);
        }

        [TestCase("table1")]
        [TestCase("schema.table1")]
        [TestCase("schema1.table")]
        [TestCase("schema1.table1")]
        public void TableMetadataCollectionNotContain(string name)
        {
            var list = this.fixture.Create<List<TableMetadata>>();
            list.Add(this.fixture
                .Build<TableMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .NotContain(name);
        }

        [Test]
        public void DatabaseMetadataCollectionContain()
        {
            var list = this.fixture.Create<List<DatabaseMetadata>>();
            list.Add(this.fixture
                .Build<DatabaseMetadata>()
                .With(x => x.DatabaseName, "database")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .Contain("database");
        }

        [Test]
        public void DatabaseMetadataCollectionNotContain()
        {
            var list = this.fixture.Create<List<DatabaseMetadata>>();
            list.Add(this.fixture
                .Build<DatabaseMetadata>()
                .With(x => x.DatabaseName, "database")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .NotContain("database1");
        }

        [TestCase("column")]
        [TestCase("table.column")]
        [TestCase("schema.table.column")]
        public void ColumnMetadataCollectionContain(string name)
        {
            var list = this.fixture.Create<List<ColumnMetadata>>();
            list.Add(this.fixture
                .Build<ColumnMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.ColumnName, "column")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .Contain(name);
        }

        [TestCase("column1")]
        [TestCase("table1.column")]
        [TestCase("table.column1")]
        [TestCase("table1.column1")]
        [TestCase("schema1.table.column")]
        [TestCase("schema.table1.column")]
        [TestCase("schema.table.column1")]
        [TestCase("schema1.table1.column")]
        [TestCase("schema1.table.column1")]
        [TestCase("schema.table1.column1")]
        [TestCase("schema1.table1.column1")]
        public void ColumnMetadataCollectionNotContain(string name)
        {
            var list = this.fixture.Create<List<ColumnMetadata>>();
            list.Add(this.fixture
                .Build<ColumnMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.ColumnName, "column")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .NotContain(name);
        }

        [TestCase("index")]
        [TestCase("table.index")]
        [TestCase("schema.table.index")]
        public void IndexMetadataCollectionContain(string name)
        {
            var list = this.fixture.Create<List<IndexMetadata>>();
            list.Add(this.fixture
                .Build<IndexMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.IndexName, "index")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .Contain(name);
        }

        [TestCase("index1")]
        [TestCase("table1.index")]
        [TestCase("table.index1")]
        [TestCase("table1.index1")]
        [TestCase("schema1.table.index")]
        [TestCase("schema.table1.index")]
        [TestCase("schema.table.index1")]
        [TestCase("schema1.table1.index")]
        [TestCase("schema1.table.index1")]
        [TestCase("schema.table1.index1")]
        [TestCase("schema1.table1.index1")]
        public void IndexMetadataCollectionNotContain(string name)
        {
            var list = this.fixture.Create<List<IndexMetadata>>();
            list.Add(this.fixture
                .Build<IndexMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.IndexName, "index")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .NotContain(name);
        }

        [TestCase("column")]
        [TestCase("index.column")]
        [TestCase("table.index.column")]
        [TestCase("schema.table.index.column")]
        public void IndexColumnMetadataCollectionContain(string name)
        {
            var list = this.fixture.Create<List<IndexColumnMetadata>>();
            list.Add(this.fixture
                .Build<IndexColumnMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.IndexName, "index")
                .With(x => x.ColumnName, "column")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .Contain(name);
        }

        [TestCase("column1")]
        [TestCase("index1.column")]
        [TestCase("table1.index.column")]
        [TestCase("table.index1.column")]
        [TestCase("table1.index1.column")]
        [TestCase("schema1.table.index.column")]
        [TestCase("schema.table1.index.column")]
        [TestCase("schema.table.index1.column")]
        [TestCase("schema1.table1.index.column")]
        [TestCase("schema1.table.index1.column")]
        [TestCase("schema.table1.index1.column")]
        [TestCase("schema1.table1.index1.column")]
        [TestCase("index1.column1")]
        [TestCase("table1.index.column1")]
        [TestCase("table.index1.column1")]
        [TestCase("table1.index1.column1")]
        [TestCase("schema1.table.index.column1")]
        [TestCase("schema.table1.index.column1")]
        [TestCase("schema.table.index1.column1")]
        [TestCase("schema1.table1.index.column1")]
        [TestCase("schema1.table.index1.column1")]
        [TestCase("schema.table1.index1.column1")]
        [TestCase("schema1.table1.index1.column1")]
        public void IndexColumnMetadataCollectionNotContain(string name)
        {
            var list = this.fixture.Create<List<IndexColumnMetadata>>();
            list.Add(this.fixture
                .Build<IndexColumnMetadata>()
                .With(x => x.TableSchema, "schema")
                .With(x => x.TableName, "table")
                .With(x => x.IndexName, "index")
                .With(x => x.ColumnName, "column")
                .Create());

            list
                .Should()
                .HaveCountGreaterThan(1)
                .And
                .NotContain(name);
        }
    }
}
