// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

using AutoFixture;

using NUnit.Framework;

namespace Gems.Linq.Tests
{
    public class EnumerableExtensionsTests
    {
        /// <summary>
        /// Проверка агрегации когда указан предикат и не нужен словарь.
        /// </summary>
        [Test]
        public void TestAggregateWithPredicateWithoutDictionary()
        {
            // Arrange
            var list = new List<TestObject>
                                    {
                                        new TestObject { Id = 1, Name = Guid.NewGuid().ToString() },
                                        new TestObject { Id = 2, Name = Guid.NewGuid().ToString() },
                                        new TestObject { Id = 3, Name = Guid.NewGuid().ToString() },
                                        new TestObject { Id = 4, Name = Guid.NewGuid().ToString() }
                                    };

            // Act
            var result = list.Aggregate(
                item => item.Id == 1 || item.Id == 8 || item.Id == 4,
                item => item.Id.ToString(),
                ",");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("1,4", result);
        }

        /// <summary>
        /// Проверка агрегации когда не указан предикат и не нужен словарь.
        /// </summary>
        [Test]
        public void TestAggregateWithoutPredicateWithoutDictionary()
        {
            // Arrange
            var list = new List<TestObject>
                       {
                           new TestObject { Id = 1, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 2, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 3, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 4, Name = Guid.NewGuid().ToString() }
                       };

            // Act
            var result = list.Aggregate(
                null,
                item => item.Id.ToString(),
                ",");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("1,2,3,4", result);
        }

        /// <summary>
        /// Проверка агрегации когда указан предикат и нужен словарь.
        /// </summary>
        [Test]
        public void TestAggregateWithPredicateWithDictionary()
        {
            // Arrange
            var list = new List<TestObject>
                       {
                           new TestObject { Id = 1, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 2, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 3, Name = Guid.NewGuid().ToString() },
                           new TestObject { Id = 4, Name = Guid.NewGuid().ToString() }
                       };

            // Act
            var result = list.AggregateWithDictionary(
                item => item.Id == 1 || item.Id == 8 || item.Id == 4,
                item => item.Id.ToString(),
                ",",
                out var dictionary);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(dictionary);
            Assert.AreEqual("1,4", result);
            Assert.True(dictionary.ContainsKey("1"));
            Assert.True(dictionary.ContainsKey("4"));
            Assert.True(dictionary.ContainsKey("2"));
            Assert.True(dictionary.ContainsKey("3"));
        }

        /// <summary>
        /// Проверка коллекции на наличие элементов.
        /// </summary>
        [Test]
        public void IsNullOrEmpty_CheckNullAndEmptyCollections_TrueResult()
        {
            // Act
            var isEmpty = new List<int>().IsNullOrEmpty();
            var nullCollectionIsEmpty = ((List<int>)null).IsNullOrEmpty();

            // Assert
            Assert.IsTrue(isEmpty);
            Assert.IsTrue(nullCollectionIsEmpty);
            Assert.Pass();
        }

        /// <summary>
        /// Проверка разбиения коллекции на подколлекции с заданным кол-вом элементов.
        /// </summary>
        [Test]
        [TestCase(1000, 10)]
        [TestCase(10_000, 1_000)]
        [TestCase(33_000, 2_000)]
        [TestCase(142_123, 3_000)]
        [TestCase(134_321, 5_000)]
        public void Tile_PackageSplitting_ShouldCorrectSplit(int sourceCount, int packageSize)
        {
            // Arrange
            var source = new Fixture().CreateMany<Guid>(sourceCount).ToList();

            // Act
            var packages = source.Tile(packageSize).ToList();
            var isSame = packages.SelectMany(item => item).SequenceEqual(source);

            // Assert
            Assert.IsNotNull(packages);
            Assert.IsNotEmpty(packages);
            Assert.IsTrue(isSame);
            Assert.Pass();
        }

        /// <summary>
        /// Проверка применения левого внешнего объединения для двух коллекций.
        /// </summary>
        [Test]
        public void LeftOuterJoin_CollectionsLeftJoin_ShouldCorrectLeftJoin()
        {
            // Arrange
            var outerCollection = new[]
            {
                new { ExternalId = 1, ExternalName = "LeftId1" },
                new { ExternalId = 2, ExternalName = "LeftId2" },
                new { ExternalId = 3, ExternalName = "LeftId3" },
                new { ExternalId = 4, ExternalName = "LeftId4" },
                new { ExternalId = 5, ExternalName = "LeftId5" }
            };
            var innerCollection = new[]
            {
                new { InnerId = 1, InnerName = "InnerId1" },
                new { InnerId = 2, InnerName = "InnerId2" },
                new { InnerId = 3, InnerName = "InnerId3" }
            };

            // Act
            var result = outerCollection
                .LeftOuterJoin(
                    innerCollection,
                    i => i.ExternalId,
                    j => j.InnerId,
                    (i, j) => new
                    {
                        Id = i.ExternalId,
                        i.ExternalName,
                        j?.InnerName
                    })
                .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result.Count(i => string.IsNullOrEmpty(i.InnerName)), 2);
            Assert.Pass();
        }

        /// <summary>
        /// Проверка применения левого внешнего объединения для двух коллекций.
        /// </summary>
        [Test]
        public void FullOuterJoin_CollectionsFullOuterJoin_ShouldCorrectFullOuterJoin()
        {
            // Arrange
            var outerCollection = new[]
            {
                new { ExternalId = 1, ExternalName = "LeftId1" },
                new { ExternalId = 2, ExternalName = "LeftId2" },
                new { ExternalId = 3, ExternalName = "LeftId3" },
                new { ExternalId = 4, ExternalName = "LeftId4" },
                new { ExternalId = 5, ExternalName = "LeftId5" }
            };
            var innerCollection = new[]
            {
                new { InnerId = 1, InnerName = "InnerId1" },
                new { InnerId = 2, InnerName = "InnerId2" },
                new { InnerId = 3, InnerName = "InnerId3" },
                new { InnerId = 6, InnerName = "InnerId4" },
                new { InnerId = 7, InnerName = "InnerId5" },
                new { InnerId = 8, InnerName = "InnerId6" }
            };

            // Act
            var result = outerCollection
                .FullOuterJoin(
                    innerCollection,
                    i => i.ExternalId,
                    j => j.InnerId,
                    (i, j, key) => new
                    {
                        Id = key,
                        i?.ExternalName,
                        j?.InnerName
                    })
                .ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 8);
            Assert.AreEqual(result.Count(i => !string.IsNullOrEmpty(i.InnerName)), 6);
            Assert.AreEqual(result.Count(i => !string.IsNullOrEmpty(i.ExternalName)), 5);
            Assert.AreEqual(result.Count(i => !string.IsNullOrEmpty(i.ExternalName) && !string.IsNullOrEmpty(i.InnerName)), 3);
            Assert.Pass();
        }

        /// <summary>
        /// Проверка удаления дубликатов по составному ключу.
        /// </summary>
        [Test]
        [Obsolete("Obsolete")]
        public void DistinctBy_DistinctElementsByComplexKey_ShouldRemoveDuplicatesByKey()
        {
            // Arrange
            var collection = new[]
            {
                new { Id = 1, Name = "Name1" },
                new { Id = 1, Name = "Name1" },
                new { Id = 2, Name = "Name2" },
                new { Id = 2, Name = "Name2" },
                new { Id = 3, Name = "Name3" },
                new { Id = 3, Name = "Name3" },
                new { Id = 4, Name = "Name4" },
                new { Id = 4, Name = "Name4" },
                new { Id = 5, Name = "Name5" },
                new { Id = 5, Name = "Name5" }
            };

            // Act
            var result = collection.DistinctBy(g => new { g.Id, g.Name }).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 5);
            Assert.Pass();
        }
    }
}
