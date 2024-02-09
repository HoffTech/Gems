// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net.Http;

namespace Gems.FeatureToggle;

public class FeatureToggleOptions
{
    public string Environment { get; set; }

    public string Url { get; set; }

    public string Token { get; set; }

    public TimeSpan FetchTogglesInterval { get; set; }

    public Func<Uri, HttpClient> CustomHttpClientBuilder { get; set; }

    public bool EnableBootstrapLoading { get; set; } = true;

    public bool SynchronousInitialization { get; set; } = true;
}
