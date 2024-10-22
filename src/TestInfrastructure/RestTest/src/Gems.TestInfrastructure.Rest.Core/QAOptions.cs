// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Rest.Core;

public class QAOptions
{
    public string Path { get; set; }

    public bool Recursive { get; set; }

    public List<string> ScopeFileNames { get; set; }

    public HttpClient HttpClient { get; set; }

    public AllureOptions Allure { get; set; }
}
