// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Patterns.SyncTables.Sample.SyncTablesWithChangeTracking.Persons.SyncPersons.Options;

public class SyncPersonsInfoOptions
{
    /// <summary>
    /// Section in appsettings.json.
    /// </summary>
    public const string SectionName = "Person:ImportPersonsFromDax:SyncPersonsInfo";

    /// <summary>
    /// Таймаут получения сущностей заказов на продажу.
    /// </summary>
    public int GetPersonsInfoTimeout { get; set; } = 90;
}
