# Утилиты для работы со схемой базы данных

Поддерживаются базы данных Postgresql и MsSql и другие провайдеры Oledb.

```csharp
public static async Task<Schema> SchemaAsync(this DbConnection connection, CancellationToken cancellationToken = default);

public static async Task<PostgresSchema> SchemaAsync(this NpgsqlConnection connection, CancellationToken cancellationToken = default);

public static async Task<MsSqlSchema> SchemaAsync(this SqlConnection connection, CancellationToken cancellationToken = default);
```

Асинхронно получает схему базы данных.

Классы `PostgresSchema` и `MsSqlSchema` наследуются от `Schema`.
Для получени текущей базы данных используйте свойство `CurrentDatabase`.


## Методы Schema

```csharp
public static UserMetadata User(this Schema schema, string name)
```

Возвращает пользователя по имени. Если пользователь не найден, то бросается исключение `InvalidOperationException`.

```csharp
public static UserMetadata TryGetUser(this Schema schema, string name)
```

Возвращает пользователя по имени. Если пользователь не найден, то возвращается `null`.

```csharp
public static DatabaseMetadata Database(this Schema schema, string name)
```

Возвращает базу данных по имени. Если база не найдена, то бросается исключение `InvalidOperationException`.

```csharp
public static DatabaseMetadata TryGetDatabase(this Schema schema, string name)
```

Возвращает базу данных по имени. Если база не найдена, то возвращается `null`.

## Методы DatabaseMetadata

```csharp
public static TableMetadata Table(this DatabaseMetadata database, string name)
```

Возвращает таблицу по имени. Если таблица не найдена, то бросается исключение `InvalidOperationException`.


```csharp
public static TableMetadata TryGetTable(this DatabaseMetadata database, string name)
```

Возвращает таблицу по имени. Если таблица не найдена, то возвращается `null`.


## Методы TableMetadata

```csharp
public static IndexMetadata Index(this TableMetadata table, string name)
```

Возвращает индекс таблицы по имени. Если индекс таблицы не найден, то бросается исключение `InvalidOperationException`.


```csharp
public static IndexMetadata TryGetIndex(this TableMetadata table, string name)
```

Возвращает индекс таблицы по имени. Если индекс таблицы не найден, то возвращается `null`.

```csharp
public static ColumnMetadata Column(this TableMetadata table, string name)
```

Возвращает колонку таблицы по имени. Если колонка таблица не найдена, то бросается исключение `InvalidOperationException`.

```csharp
public static ColumnMetadata TryGetColumn(this TableMetadata table, string name)
```

Возвращает колонку таблицы по имени. Если колонка таблицы не найдена, то возвращается `null`.



## Методы IndexMetadata

```csharp
public static IndexColumnMetadata Column(this IndexMetadata table, string name)
```

Возвращает колонку индекса по имени. Если колонка индекса не найдена, то бросается исключение `InvalidOperationException`.


```csharp
public static IndexColumnMetadata TryGetColumn(this IndexMetadata table, string name)
```

Возвращает колонку индекса по имени. Если колонка индекса не найдена, то возвращается `null`.

