// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gems.Linq
{
    /// <summary>
    /// Добавляет методы расширения для работы с перечисляемой коллекцией.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Проверяет коллекцию на null и на наличие элементов.
        /// </summary>
        /// <param name="source">Коллекция источник.</param>
        /// <typeparam name="TSource">Тип коллекции иточника.</typeparam>
        /// <returns>Признак пустой коллекции или null.</returns>
        /// <example>
        /// <code>
        /// public void Method(IEnumerable collection)
        /// {
        ///    //...
        ///    if(collection.IsNullOrEmpty())
        ///    {
        ///        // do some logic...
        ///    }
        /// }
        /// </code>
        /// </example>
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source is null || !source.Any();
        }

        /// <summary>
        /// Разбивает коллекцию источник на несколько коллекций по величине количества элементов.
        /// </summary>
        /// <param name="source">Коллекция источник.</param>
        /// <param name="batchSize">Величина пакета.</param>
        /// <typeparam name="TSource">Тип коллекции источника.</typeparam>
        /// <returns>Список коллекций после проведения разбиения.</returns>
        /// <example>
        /// <code>
        /// public void Method(IEnumerable sourceCollection)
        /// {
        ///     var packages = sourceCollection.Tile(packageSize);
        ///     //do some logic with packages...
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<List<TSource>> Tile<TSource>(this IEnumerable<TSource> source, int batchSize)
        {
            return source
                .Select((x, index) => new { Index = index, Item = x })
                .GroupBy(x => Math.DivRem(x.Index, batchSize, out _), x => x.Item)
                .OrderBy(g => g.Key)
                .Select(g => g.ToList());
        }

        /// <summary>
        /// Применение левого внешнего объединения для двух коллекций.
        /// </summary>
        /// <param name="outer">Внешняя коллекция.</param>
        /// <param name="inner">Внутренняя коллекция.</param>
        /// <param name="outerKeySelector">Поле для установки связи с внешней коллекцией.</param>
        /// <param name="innerKeySelector">Поле для установки связи с внутренней коллекцией.</param>
        /// <param name="resultSelector">Единица данных в результате установки связки.</param>
        /// <typeparam name="TOuter">Тип данных внешней коллекции.</typeparam>
        /// <typeparam name="TInner">Тип данных внутренней коллекции.</typeparam>
        /// <typeparam name="TKey">Ключ.</typeparam>
        /// <typeparam name="TResult">Результирующий тип данных.</typeparam>
        /// <example>
        /// <code>
        ///     public IEnumerable DoLeftOuterJoin(IEnumerable sourceCollection, IEnumerable joinCollection)
        ///     {
        ///         return sourceCollection
        ///             .LeftOuterJoin(
        ///             joinCollection,
        ///                 sc => sc.Id,
        ///                 jc => jc.Id,
        ///                 (sc, jc) => new Model
        ///                 {
        ///                     ModelId = sc.Id,
        ///                     ExternalId = jc.Id,
        ///                     ...other fields
        ///                 });
        ///     }
        /// </code>
        /// </example>
        /// <returns>Коллекция результат.</returns>
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => new
                {
                    a,
                    b
                })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => resultSelector(x.a, b!));
        }

        /// <summary>
        /// Применение полного внешнего объединения для двух коллекций.
        /// </summary>
        /// <param name="outer">Внешняя коллекция.</param>
        /// <param name="inner">Внутренняя коллекция.</param>
        /// <param name="outerKeySelector">Ключ для установки связи с внешней коллекцией.</param>
        /// <param name="innerKeySelector">Ключ для установки связи с внутренней коллекцией.</param>
        /// <param name="resultSelector">Единица данных в результате установки связки.</param>
        /// <param name="defaultOuter">Значение по умолчанию для внешней коллекции если равно null.</param>
        /// <param name="defaultInner">Значение по умолчанию для внутренней коллекции если равно null.</param>
        /// <param name="comparer">Компаратор.</param>
        /// <typeparam name="TOuter">Тип элемента данных внешней коллекции.</typeparam>
        /// <typeparam name="TInner">Тип элемента данных внутренней коллекции.</typeparam>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <typeparam name="TResult">Тип результата.</typeparam>
        /// <example>
        /// <code>
        ///     public IEnumerable DoFullOuterJoin(IEnumerable outerCollection, IEnumerable innerCollection)
        ///     {
        ///         return outerCollection
        ///             .LeftOuterJoin(
        ///                 innerCollection,
        ///                 sc => o.Id,
        ///                 jc => i.Id,
        ///                 (sc, jc, key) => new Model
        ///                 {
        ///                     ModelId = key,
        ///                     Field1 = o?.Field1,
        ///                     Field2 = i?.Field2,
        ///                     ...other fields
        ///                 });
        ///     }
        /// </code>
        /// </example>
        /// <returns>Коллекция в результате полного объединения двух коллекций.</returns>
        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TKey, TResult> resultSelector,
            TOuter defaultOuter = default,
            TInner defaultInner = default,
            IEqualityComparer<TKey> comparer = null)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            var outerLookup = outer.ToLookup(outerKeySelector, comparer);
            var innerLookup = inner.ToLookup(innerKeySelector, comparer);

            var keys = new HashSet<TKey>(outerLookup.Select(p => p.Key), comparer);
            keys.UnionWith(innerLookup.Select(p => p.Key));

            var join = from key in keys
                       from outerValue in outerLookup[key].DefaultIfEmpty(defaultOuter)
                       from innerValue in innerLookup[key].DefaultIfEmpty(defaultInner)
                       select resultSelector(outerValue, innerValue, key);

            return join;
        }

        /// <summary>
        /// Удаляет дубликаты по составному ключу.
        /// </summary>
        /// <param name="source">Внешняя коллекция.</param>
        /// <param name="selector">Ключ.</param>
        /// <typeparam name="TSource">Тип элемента данных внешней коллекции.</typeparam>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <example>
        /// <code>
        ///     public IEnumerable DoDistinctBy(IEnumerable collection)
        ///     {
        ///         return collection.DistinctBy(g => new { g.Id, g.Name });
        ///     }
        /// </code>
        /// </example>
        /// <returns>Коллекция без дубликатов по ключу.</returns>
        [Obsolete("Используй System.Linq")]
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(selector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Запускает цикл foreach с предварительной проверкой коллекции на null.
        /// </summary>
        /// <param name="enumerable">коллекция к итерации.</param>
        /// <param name="action">действие.</param>
        /// <typeparam name="T">тип элементов в коллекции.</typeparam>
        public static void SafeForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable is not null)
            {
                foreach (var element in enumerable)
                {
                    action(element);
                }
            }
        }

        public static string Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, string> keyFunc, string divider)
        {
            return AggregateInternal(source, predicate, keyFunc, divider, out _);
        }

        public static string AggregateWithDictionary<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, string> keyFunc, string divider, out IDictionary<string, TSource> dictionary)
        {
            return AggregateInternal(source, predicate, keyFunc, divider, out dictionary);
        }

        private static string AggregateInternal<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, string> keyFunc, string divider, out IDictionary<string, TSource> dictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keyFunc == null)
            {
                throw new ArgumentNullException(nameof(keyFunc));
            }

            if (string.IsNullOrEmpty(divider))
            {
                throw new ArgumentNullException(nameof(divider));
            }

            dictionary = new Dictionary<string, TSource>();

            var filteredPredicate = predicate ?? (_ => true);

            var result = string.Empty;

            foreach (var item in source.Where(filteredPredicate))
            {
                result += $"{keyFunc(item)}{divider}";

                dictionary.TryAdd(keyFunc(item), item);
            }

            if (string.IsNullOrEmpty(result))
            {
                result = result[..^divider.Length];
            }

            return result;
        }
    }
}
