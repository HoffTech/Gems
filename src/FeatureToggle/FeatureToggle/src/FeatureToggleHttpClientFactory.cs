// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Net.Http;

using Unleash;

namespace Gems.FeatureToggle;

internal class FeatureToggleHttpClientFactory : IHttpClientFactory
{
    private readonly Func<Uri, HttpClient> buildHttpClient;

    public FeatureToggleHttpClientFactory(Func<Uri, HttpClient> buildHttpClient)
    {
        this.buildHttpClient = buildHttpClient;
    }

    public HttpClient Create(Uri unleashApiUri)
    {
        return this.buildHttpClient(unleashApiUri);
    }
}
