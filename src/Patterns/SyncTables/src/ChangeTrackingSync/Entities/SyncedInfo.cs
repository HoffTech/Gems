// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Data.Npgsql;

using NpgsqlTypes;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

/// <summary>
/// Информация о последней выполненной синхронизации.
/// </summary>
[PgType("t_change_tracking_info")]
public class SyncedInfo
{
    /// <summary>
    /// Название целевой таблицы.
    /// </summary>
    [PgName("table_name")]
    public string TableName { get; set; }

    /// <summary>
    /// Версия до которой выполненна синхронизация.
    /// </summary>
    [PgName("ct_version")]
    public long Version { get; set; }

    /// <summary>
    /// Дата последнего восстановления backup`a бд источника на момент синхронизации.
    /// </summary>
    [PgName("last_restore_datetime")]
    public DateTime? LastRestoreDateTime { get; set; }

    /// <summary>
    /// Время сохранения информации о синхронизации.
    /// </summary>
    [PgName("update_time")]
    public DateTime UpdateTime { get; set; }
}
