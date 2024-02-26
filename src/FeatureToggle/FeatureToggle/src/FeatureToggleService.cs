// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Unleash;
using Unleash.ClientFactory;
using Unleash.Events;
using Unleash.Internal;

namespace Gems.FeatureToggle;

public class FeatureToggleService : IFeatureToggleService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IOptions<FeatureToggleOptions> featureToggleOptions;
    private readonly ILogger<FeatureToggleService> logger;

    private IUnleash featureToggleClient;
    private IUnleashContextProvider contextProvider;

    public FeatureToggleService(
        IServiceProvider serviceProvider,
        IOptions<FeatureToggleOptions> featureToggleOptions,
        ILogger<FeatureToggleService> logger,
        Type[] featureTogglesTypes)
    {
        this.serviceProvider = serviceProvider;
        this.featureToggleOptions = featureToggleOptions;
        this.logger = logger;
        this.HolderTypes = featureTogglesTypes;
    }

    public Type[] HolderTypes { get; }

    public Dictionary<string, bool> FeatureToggles
    {
        get
        {
            if (this.featureToggleClient?.FeatureToggles == null)
            {
                return new Dictionary<string, bool>();
            }

            return this.featureToggleClient.FeatureToggles
                .GroupBy(g => g.Name)
                .ToDictionary(
                    x => x.Key,
                    v => v.First().Enabled);
        }
    }

    public bool IsEnabled(string toggleName, bool defaultValue = false)
    {
        if (this.featureToggleClient == null
            || this.contextProvider == null)
        {
            throw new ArgumentException("client uninitialized");
        }

        return this.featureToggleClient.IsEnabled(toggleName, defaultValue);
    }

    public bool IsEnabled(
        string toggleName,
        Dictionary<string, string> contextProperty,
        bool defaultValue = false)
    {
        if (this.featureToggleClient == null
            || this.contextProvider == null)
        {
            throw new ArgumentException("client uninitialized");
        }

        var context = this.BuildUnleashContext(contextProperty);

        return this.featureToggleClient.IsEnabled(toggleName, context, defaultValue);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return this.InitializeClient();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.featureToggleClient?.Dispose();
        return Task.CompletedTask;
    }

    private async Task InitializeClient()
    {
        var settings = new UnleashSettings
        {
            AppName = this.featureToggleOptions.Value.Environment,
            UnleashApi = new Uri(this.featureToggleOptions.Value.Url),
            InstanceTag = this.featureToggleOptions.Value.Token,
            FetchTogglesInterval = this.featureToggleOptions.Value.FetchTogglesInterval,
            HttpClientFactory = this.featureToggleOptions.Value.CustomHttpClientBuilder != null
                ? new FeatureToggleHttpClientFactory(this.featureToggleOptions.Value.CustomHttpClientBuilder)
                : new DefaultHttpClientFactory(),
            BootstrapOverride = this.featureToggleOptions.Value.EnableBootstrapLoading
        };

        this.contextProvider = settings.UnleashContextProvider;

        this.featureToggleClient = await new UnleashClientFactory().CreateClientAsync(
            settings,
            synchronousInitialization: this.featureToggleOptions.Value.SynchronousInitialization);

        this.featureToggleClient.ConfigureEvents(c =>
        {
            c.ErrorEvent = this.ProcessError;
            c.ImpressionEvent = this.ProcessImpression;
            c.TogglesUpdatedEvent = this.ProcessTogglesUpdated;
        });

        // initial updates
        this.logger.LogInformation("FeatureToggles initial read on: {updatedOn}", DateTime.UtcNow);
        this.ProcessTogglesUpdated();
    }

    private UnleashContext BuildUnleashContext(Dictionary<string, string> contextProperty)
    {
        var defaultContext = this.contextProvider.Context;

        if (contextProperty.TryGetValue("userId", out var userId))
        {
            defaultContext.UserId = userId;
        }
        else if (contextProperty.TryGetValue("sessionId", out var sessionId))
        {
            defaultContext.SessionId = sessionId;
        }

        var context = new UnleashContext(
            defaultContext.AppName,
            defaultContext.Environment,
            defaultContext.UserId,
            defaultContext.SessionId,
            defaultContext.RemoteAddress,
            DateTimeOffset.UtcNow,
            contextProperty);
        return context;
    }

    private void ProcessTogglesUpdated(TogglesUpdatedEvent updated)
    {
        this.logger.LogInformation("FeatureToggles updated on: {updatedOn}", updated.UpdatedOn);
        this.ProcessTogglesUpdated();
    }

    private void ProcessTogglesUpdated()
    {
        // get all bool properties in objects marked with FeatureTogglesAttribute
        var featureToggleObjectsProperties = this.GetFeatureToggleObjectsProperties();

        // prepare data from server for mapping with convention
        // match by name:
        // - case insensitive,
        // - snake_case_will_be ignored
        var conventionProperties = this.featureToggleClient!.FeatureToggles
            .GroupBy(g =>
                g.Name.Replace("_", string.Empty).ToLowerInvariant())
            .ToDictionary(
                g => g.Key,
                g => g.FirstOrDefault());

        foreach (var featureToggleObjectProperty in featureToggleObjectsProperties)
        {
            // try to map properties with data from FeatureToggleAttribute
            if (this.TrySetValueByAttributeMapping(featureToggleObjectProperty, out var defaultValue))
            {
                continue;
            }

            // try set value using naming convention
            if (conventionProperties.TryGetValue(featureToggleObjectProperty.info.Name.ToLowerInvariant(), out var featureToggleByConvention))
            {
                this.logger.LogTrace(featureToggleByConvention!.ToString());
                featureToggleObjectProperty.info.SetValue(featureToggleObjectProperty.holderObject, featureToggleByConvention.Enabled);
            }
            else
            {
                // use default value
                this.logger.LogInformation(
                    "Not found matched feature toggle for: {name}. " +
                    "Setting default value {defaultValue}.",
                    featureToggleObjectProperty.info.Name,
                    defaultValue);
                featureToggleObjectProperty.info.SetValue(featureToggleObjectProperty.holderObject, defaultValue);
            }
        }

        this.logger.LogInformation(
            "Updated feature toggles: {data}",
            JsonConvert.SerializeObject(
                featureToggleObjectsProperties
                    .Select(ftp => ftp.holderObject)
                    .Distinct()
                    .ToArray()));
    }

    private bool TrySetValueByAttributeMapping(
        (object holderObject, PropertyInfo info) featureToggleObjectProperty,
        out bool defaultValue)
    {
        defaultValue = false;

        var featureToggleAttrubute = featureToggleObjectProperty.info
            .GetCustomAttributes(typeof(FeatureToggleAttribute), false)
            .OfType<FeatureToggleAttribute>()
            .FirstOrDefault();

        if (featureToggleAttrubute == null)
        {
            return false;
        }

        defaultValue = featureToggleAttrubute.DefaultValue;

        if (string.IsNullOrWhiteSpace(featureToggleAttrubute.FeatureName))
        {
            return false;
        }

        var featureToggleByAttribute = this.featureToggleClient.FeatureToggles
            .FirstOrDefault(ft =>
                ft.Name.Equals(featureToggleAttrubute.FeatureName));

        if (featureToggleByAttribute != null)
        {
            featureToggleObjectProperty.info.SetValue(featureToggleObjectProperty.holderObject, featureToggleByAttribute.Enabled);
            return true;
        }

        this.logger.LogTrace(
            "Not found matched feature toggle for: {featureName} specified in attributes. " +
            "Trying to search by name convention or use default value",
            featureToggleAttrubute.FeatureName);

        return false;
    }

    private (object holderObject, PropertyInfo info)[] GetFeatureToggleObjectsProperties()
    {
        var featureToggleProperties = this.HolderTypes
            .Select(ftt => new
            {
                holderType = ftt,
                holderObject = this.serviceProvider.GetRequiredService(ftt)
            })
            .SelectMany(o => o.holderType
                .GetProperties()
                .Where(p => p.PropertyType == typeof(bool))
                .Select(op => (o.holderObject, op)))
            .ToArray();

        return featureToggleProperties;
    }

    private void ProcessImpression(ImpressionEvent evt)
    {
        this.logger.LogTrace("type: {type} name:{featureName} isEnabled:{enabled}", evt.Type, evt.FeatureName, evt.Enabled);
    }

    private void ProcessError(ErrorEvent errorEvent)
    {
        this.logger.LogError(
            "FeatureToggle error: {error} resource: {resource}",
            errorEvent.Error,
            errorEvent.Resource);
    }
}
