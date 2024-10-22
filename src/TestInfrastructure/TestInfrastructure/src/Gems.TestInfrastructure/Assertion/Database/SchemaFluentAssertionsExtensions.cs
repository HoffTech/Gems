// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using FluentAssertions;
using FluentAssertions.Collections;

using Gems.TestInfrastructure.Utils.Database;

namespace Gems.TestInfrastructure.Assertion.Database
{
    public static class SchemaFluentAssertionsExtensions
    {
        public static AndWhichConstraint<GenericCollectionAssertions<TableMetadata>, TableMetadata> Contain(
            this GenericCollectionAssertions<TableMetadata> tables,
            string name)
        {
            return tables.Contain(x => x.Equals(name));
        }

        public static AndWhichConstraint<GenericCollectionAssertions<ColumnMetadata>, ColumnMetadata> Contain(
            this GenericCollectionAssertions<ColumnMetadata> columns,
            string name)
        {
            return columns.Contain(x => x.Equals(name));
        }

        public static AndWhichConstraint<GenericCollectionAssertions<UserMetadata>, UserMetadata> Contain(
            this GenericCollectionAssertions<UserMetadata> users,
            string name)
        {
            return users.Contain(x => x.UserName.Equals(name));
        }

        public static AndWhichConstraint<GenericCollectionAssertions<DatabaseMetadata>, DatabaseMetadata> Contain(
            this GenericCollectionAssertions<DatabaseMetadata> databases,
            string name)
        {
            return databases.Contain(x => x.DatabaseName.Equals(name));
        }

        public static AndWhichConstraint<GenericCollectionAssertions<IndexMetadata>, IndexMetadata> Contain(
            this GenericCollectionAssertions<IndexMetadata> indexes,
            string name)
        {
            return indexes.Contain(x => x.Equals(name));
        }

        public static AndWhichConstraint<GenericCollectionAssertions<IndexColumnMetadata>, IndexColumnMetadata> Contain(
            this GenericCollectionAssertions<IndexColumnMetadata> indexColumns,
            string name)
        {
            return indexColumns.Contain(x => x.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<TableMetadata>> NotContain(
            this GenericCollectionAssertions<TableMetadata> tables,
            string name)
        {
            return tables.NotContain(x => x.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<ColumnMetadata>> NotContain(
            this GenericCollectionAssertions<ColumnMetadata> columns,
            string name)
        {
            return columns.NotContain(x => x.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<UserMetadata>> NotContain(
            this GenericCollectionAssertions<UserMetadata> users,
            string name)
        {
            return users.NotContain(x => x.UserName.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<DatabaseMetadata>> NotContain(
            this GenericCollectionAssertions<DatabaseMetadata> databases,
            string name)
        {
            return databases.NotContain(x => x.DatabaseName.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<IndexMetadata>> NotContain(
            this GenericCollectionAssertions<IndexMetadata> indexes,
            string name)
        {
            return indexes.NotContain(x => x.Equals(name));
        }

        public static AndConstraint<GenericCollectionAssertions<IndexColumnMetadata>> NotContain(
            this GenericCollectionAssertions<IndexColumnMetadata> indexColumns,
            string name)
        {
            return indexColumns.NotContain(x => x.Equals(name));
        }
    }
}
