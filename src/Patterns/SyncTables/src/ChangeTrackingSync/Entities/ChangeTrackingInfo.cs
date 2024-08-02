// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Patterns.SyncTables.ChangeTrackingSync.Entities;

/// <summary>
/// Информация о состоянии ChangeTracking'a в базе источнике.
/// </summary>
public class ChangeTrackingInfo
{
    /// <summary>
    /// Минимальная версия доступных изменений в истории.
    /// </summary>
    public long? MinValidVersion { get; set; }

    /// <summary>
    /// Текущая версия CT изменений. Версия последней зафиксированной транзакции.
    /// </summary>
    public long? CurrentVersion { get; set; }

    /// <summary>
    /// Дата последнего восстановления БД из backup.
    /// </summary>
    public DateTime? LastRestoreDateTime { get; set; }

    /// <summary>
    /// Текущее максимальное значение PrimaryKey.
    /// </summary>
    public long? MaxKey { get; set; }

    /// <summary>
    /// Текущее минимальное значение PrimaryKey.
    /// </summary>
    public long? MinKey { get; set; }
}
