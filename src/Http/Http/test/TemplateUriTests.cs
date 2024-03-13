// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using NUnit.Framework;

namespace Gems.Http.Tests;

public class TemplateUriTests
{
    /// <summary>
    /// Возвращает uri без обработки, если uri равен null или пустой строке.
    /// </summary>
    /// <param name="uri">uri.</param>
    [TestCase(null)]
    [TestCase("")]
    public void GetUri_TemplateUriIsEmptyOrNull_ReturnsNullOrEmpty(string uri)
    {
        var templateUri = new TemplateUri(uri);
        Assert.AreEqual(uri, templateUri.GetUri());
    }

    /// <summary>
    /// Возвращает uri без обработки, если uri не имеет плейсхолдеров.
    /// </summary>
    /// <param name="uri">uri.</param>
    [TestCase("http://someapi/v1/somauri")]
    [TestCase("http://someapi/v1/somauri?someparamter=somevalue")]
    [TestCase("/v1/somauri")]
    [TestCase("/v1/somauri?someparamter=somevalue")]
    [TestCase("v1/somauri")]
    [TestCase("v1/somauri?someparamter=somevalue")]
    public void GetUri_TemplateUriHasNoPlaceholders_ReturnsTheSameUri(string uri)
    {
        var templateUri = new TemplateUri(uri);
        Assert.AreEqual(uri, templateUri.GetUri());
    }

    /// <summary>
    /// Бросает исключение если uri содержит плейсхолдеры, а аргументы не были переданы в TemplateUri.
    /// </summary>
    /// <param name="uri">uri.</param>
    [TestCase("v1/orders/{orderId}")]
    [TestCase("v1/users/{userId}/orders/{orderId}")]
    public void GetUri_TemplateArgsIsNullEmpty_ThrowArgumentException(string uri)
    {
        var templateUri = new TemplateUri(uri);
        var exception = Assert.Throws<ArgumentException>(() => templateUri.GetUri());
        Assert.NotNull(exception);
        Assert.AreEqual(TemplateUri.TemplateArgsArgumentErrorMessage, exception.Message);
    }

    /// <summary>
    /// Бросает исключение если uri содержит не корректный плейсхолдер.
    /// </summary>
    /// <param name="uri">uri.</param>
    /// <param name="arg1">arg1.</param>
    [TestCase("v1/orders/{orderId", "123")]
    [TestCase("v1/orders/{orderId?someparamter=somevalue", "456")]
    public void GetUri_PlaceholderIsIncorrect_ThrowArgumentException(string uri, string arg1)
    {
        var templateUri = new TemplateUri(uri, arg1);
        var exception = Assert.Throws<ArgumentException>(() => templateUri.GetUri());
        Assert.NotNull(exception);
        var templateUriWithoutQueryString = uri.Split('?')[0];
        Assert.AreEqual(string.Format(TemplateUri.IncorrectPlaceholderErrorMessage, templateUriWithoutQueryString), exception.Message);
    }

    /// <summary>
    /// Бросает исключение если массив с ааргументами содержит не все значения для замены плейсхолдеров.
    /// </summary>
    /// <param name="uri">uri.</param>
    /// <param name="arg1">arg1.</param>
    [TestCase("v1/users/{userId}/orders/{orderId}", "rak")]
    [TestCase("v1/users/{userId}/orders/{orderId}?someparamter=somevalue", "rak")]
    public void GetUri_TemplateArgsContainsNotAllPlaceholders_ThrowArgumentException(string uri, string arg1)
    {
        var templateUri = new TemplateUri(uri, arg1);
        var exception = Assert.Throws<ArgumentException>(() => templateUri.GetUri());
        Assert.NotNull(exception);
        var templateUriWithoutQueryString = uri.Split('?')[0];
        Assert.AreEqual(string.Format(TemplateUri.TemplateArgsContainsNotAllPlaceholdersErrorMessage, arg1, templateUriWithoutQueryString), exception.Message);
    }
}
