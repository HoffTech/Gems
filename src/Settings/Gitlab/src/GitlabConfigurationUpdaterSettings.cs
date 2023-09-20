// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

namespace Gems.Settings.Gitlab
{
    public class GitlabConfigurationUpdaterSettings : GitlabConfigurationSettings
    {
        public TimeSpan UpdateInterval { get; set; } = new TimeSpan(0, 5, 0);

        public Action<IServiceProvider, Exception> HandleError { get; set; } = null;

        public Action<IServiceProvider, string, string, string> ValueChanged { get; set; } = null;

        public DateTime LastUpdate { get; set; }

        public bool? LastUpdateSucceeded { get; set; }
    }
}
