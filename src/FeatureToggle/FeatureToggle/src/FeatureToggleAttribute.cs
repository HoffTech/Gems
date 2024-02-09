// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.FeatureToggle;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FeatureToggleAttribute : Attribute
{
    public FeatureToggleAttribute(string featureName, bool defaultValue = false)
    {
        this.FeatureName = featureName;
        this.DefaultValue = defaultValue;
    }

    public FeatureToggleAttribute(bool defaultValue)
    {
        this.DefaultValue = defaultValue;
        this.FeatureName = null;
    }

    public string FeatureName { get; }

    public bool DefaultValue { get; }
}
