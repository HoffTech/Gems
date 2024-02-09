// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;

namespace Gems.FeatureToggle;

public interface IFeatureToggleService : IHostedService
{
    public Type[] HolderTypes { get; }

    public Dictionary<string, bool> FeatureToggles { get; }

    public bool IsEnabled(string toggleName, bool defaultValue = false);

    public bool IsEnabled(
        string toggleName,
        Dictionary<string, string> contextProperty,
        bool defaultValue = false);
}
