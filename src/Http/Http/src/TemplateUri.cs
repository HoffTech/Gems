// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.RegularExpressions;

namespace Gems.Http;

public class TemplateUri
{
    private readonly string templateUri;
    private readonly string[] templateArgs;

    public TemplateUri(string templateUri, string[] templateArgs)
    {
        this.templateUri = templateUri;
        this.templateArgs = templateArgs;
    }

    public string GetTemplateUri()
    {
        return this.templateUri;
    }

    public string GetUri()
    {
        if (string.IsNullOrEmpty(this.templateUri))
        {
            return this.templateUri;
        }

        var templateUriParts = this.templateUri.Split('?');
        var templateUriWithoutQueryString = templateUriParts[0];

        if (templateUriWithoutQueryString.IndexOf("{", StringComparison.InvariantCulture) == -1)
        {
            return this.templateUri;
        }

        if (this.templateArgs == null || this.templateArgs.Length == 0)
        {
            throw new ArgumentNullException($"Пустой массив {nameof(this.templateArgs)}");
        }

        var placeholdersMatches = Regex.Matches(templateUriWithoutQueryString, "{[^}]*}");
        if (placeholdersMatches.Count == 0)
        {
            throw new ArgumentNullException($"Не найдены плейсходеры в {nameof(this.templateUri)}: {templateUriWithoutQueryString}");
        }

        var templateUriWithPositionPlaceholders = templateUriWithoutQueryString;
        for (var i = 0; i < placeholdersMatches.Count; i++)
        {
            templateUriWithPositionPlaceholders = templateUriWithPositionPlaceholders.Replace(placeholdersMatches[i].Value, $"{{{i}}}");
        }

        for (var i = 0; i < this.templateArgs.Length; i++)
        {
            templateUriWithPositionPlaceholders = templateUriWithPositionPlaceholders.Replace($"{{{i}}}", this.templateArgs[i]);
        }

        if (templateUriWithPositionPlaceholders.IndexOf("{", StringComparison.InvariantCulture) >= 0)
        {
            throw new ArgumentNullException($"Массив {nameof(this.templateArgs)}: {string.Join(",", this.templateArgs)} возвращает не все аргументы, необходимые для замены параметров в {nameof(this.templateUri)}: {templateUriWithoutQueryString}");
        }

        templateUriParts[0] = templateUriWithPositionPlaceholders;
        return string.Join("?", templateUriParts);
    }
}

public static class TemplateUriExtensions
{
    public static TemplateUri ToTemplateUri(this string uri, params string[] args)
    {
        return new TemplateUri(uri, args);
    }
}
