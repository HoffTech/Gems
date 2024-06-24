// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Jobs.Quartz.Configuration;

public class TriggersFromDbOptions
{
    public string TriggerName { get; set; }

    /// <summary>
    /// Значение по умолчанию, на случай отсутствия значения в провайдере
    /// </summary>
    public string CronExpression { get; set; }

    public string ProviderType { get; set; }
}
