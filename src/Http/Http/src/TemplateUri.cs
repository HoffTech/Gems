// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;
using System.Text.RegularExpressions;

namespace Gems.Http;

public class TemplateUri
{
    public const string TemplateArgsArgumentErrorMessage = $"Пустой массив {nameof(templateArgs)}";
    public const string IncorrectPlaceholderErrorMessage = $"Не корректный плейсходер в {nameof(templateUri)}: {{0}}";
    public const string TemplateArgsContainsNotAllPlaceholdersErrorMessage = $"Массив {nameof(templateArgs)}: {{0}} содержит не все аргументы, необходимые для замены плейсхолдеров в {nameof(templateUri)}: {{1}}";
    private readonly string templateUri;
    private readonly string[] templateArgs;

    public TemplateUri(string templateUri, params string[] templateArgs)
    {
        this.templateUri = templateUri;
        this.templateArgs = templateArgs;
    }

    public string GetUriWithoutQueryString()
    {
        if (string.IsNullOrWhiteSpace(this.templateUri))
        {
            return null;
        }

        if (this.templateUri.IndexOf("?", StringComparison.Ordinal) == -1)
        {
            return this.templateUri;
        }

        return this.templateUri.Split('?')[0];
    }

    public string GetUri()
    {
        if (string.IsNullOrEmpty(this.templateUri))
        {
            return this.templateUri;
        }

        if (this.templateUri.IndexOf("{", StringComparison.InvariantCulture) == -1)
        {
            return this.templateUri;
        }

        if (this.templateArgs == null || this.templateArgs.Length == 0)
        {
            throw new ArgumentException(TemplateArgsArgumentErrorMessage);
        }

        var placeholdersMatches = Regex.Matches(this.templateUri, "{[^}]*}");
        if (placeholdersMatches.Count == 0)
        {
            throw new ArgumentException(string.Format(IncorrectPlaceholderErrorMessage, this.templateUri));
        }

        var templateUriWithPositionPlaceholders = this.templateUri;
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
            throw new ArgumentException(string.Format(TemplateArgsContainsNotAllPlaceholdersErrorMessage, string.Join(',', this.templateArgs), this.templateUri));
        }

        return templateUriWithPositionPlaceholders;
    }
}

public static class TemplateUriExtensions
{
    public static TemplateUri ToTemplateUri(this string uri, params string[] args)
    {
        return new TemplateUri(uri, args);
    }
}
