// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.Settings.Gitlab.Configuration;

public class GitlabSettingsConfiguration
{
    public bool EnableBackgroundUpdater { get; set; } = true;

    public bool EnableEndpointUpdater { get; set; } = false;
}
