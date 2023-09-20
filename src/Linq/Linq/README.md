# Gems.Linq

Эта библиотека .NET предоставляет различные расширения для работы с Linq. 

Библиотека предназначена для следующих сред выполнения (и новее):

* .NET 6.0

# Содержание

* [Установка и настройка](#установка)
* [Проверка коллекции на null и на наличие элементов](#проверка-коллекции-на-null-и-на-наличие-элементов)
* [Разбить коллекцию источник на несколько коллекций по величине количества элементов](#разбить-коллекцию-источник-на-несколько-коллекций-по-величине-количества-элементов)
* [Применение левого внешнего объединения для двух коллекций](#применение-левого-внешнего-объединения-для-двух-коллекций)
* [Применение полного внешнего объединения для двух коллекций](#применение-полного-внешнего-объединения-для-двух-коллекций)
* [Удалить дубликаты по составному ключу](#удалить-дубликаты-по-составному-ключу)

# Установка
- Установите nuget пакет **Gems.Linq** через менеджер пакетов.

# Проверка коллекции на null и на наличие элементов

bool IsNullOrEmpty<TSource>(_this IEnumerable<TSource> source_):
```csharp
    public void Method(IEnumerable collection)
    {
        //...
        if(collection.IsNullOrEmpty())
        {
            // do some logic...
        }
    }
```

# Разбить коллекцию источник на несколько коллекций по величине количества элементов
IEnumerable<List<TSource>> Tile<TSource>(_this IEnumerable<TSource> source, int batchSize_):
```csharp
    public void Method(IEnumerable sourceCollection)
    {
        var packages = sourceCollection.Tile(packageSize);
        //do some logic with packages...
    }
```

# Применение левого внешнего объединения для двух коллекций
IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
  this IEnumerable<TOuter> outer,
  IEnumerable<TInner> inner,
  Func<TOuter, TKey> outerKeySelector,
  Func<TInner, TKey> innerKeySelector,
  Func<TOuter, TInner, TResult> resultSelector):
```csharp
    public IEnumerable DoLeftOuterJoin(IEnumerable sourceCollection, IEnumerable joinCollection)
    {
        return sourceCollection
            .LeftOuterJoin(
            joinCollection,
            sc => sc.Id,
            jc => jc.Id,
            (sc, jc) => new Model
            {
                ModelId = sc.Id,
                ExternalId = jc.Id,
                ...other fields
            });
    }
```

# Применение полного внешнего объединения для двух коллекций
IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(
  this IEnumerable<TOuter> outer,
  IEnumerable<TInner> inner,
  Func<TOuter, TKey> outerKeySelector,
  Func<TInner, TKey> innerKeySelector,
  Func<TOuter, TInner, TKey, TResult> resultSelector,
  TOuter defaultOuter = default,
  TInner defaultInner = default,
  IEqualityComparer<TKey> comparer = null):
```csharp
    public IEnumerable DoFullOuterJoin(IEnumerable outerCollection, IEnumerable innerCollection)
    {
        return outerCollection
            .LeftOuterJoin(
                innerCollection,
                sc => o.Id,
                jc => i.Id,
                (sc, jc, key) => new Model
                {
                    ModelId = key,
                    Field1 = o?.Field1,
                    Field2 = i?.Field2,
                    ...other fields
                });
         }
    }
```

# Удалить дубликаты по составному ключу
IEnumerable<TSource> DistinctBy<TSource, TKey>(
  this IEnumerable<TSource> source,
  Func<TSource, TKey> keySelector):
```csharp
    public IEnumerable DoDistinctBy(IEnumerable collection)
    {
        return collection.DistinctBy(g => new { g.Id, g.Name });
    }
``` 

# Запустить цикл foreach с предварительной проверкой коллекции на null
void SafeForEach<T>(this IEnumerable<T> enumerable, Action<T> action):
```csharp
    public IEnumerable DoForeachOnNestedCollections(Container container)
    {
            container.Repeats.SafeForEach(repeat =>
            {
                repeat.Items.SafeForEach(ValidateContentForNullValues);
            });

            container.Tables.SafeForEach(table =>
            {
                table.Rows.SafeForEach(ValidateContentForNullValues);
            });

            container.Lists.SafeForEach(list =>
            {
                list.Items.SafeForEach(item =>
                {
                    item.NestedFields.SafeForEach(ValidateContentForNullValues);
                    ValidateContentForNullValues(item);
                });
            });
    }
``` 