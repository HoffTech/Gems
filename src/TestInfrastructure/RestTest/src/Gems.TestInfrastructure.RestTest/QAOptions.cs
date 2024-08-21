// <copyright file="QAOptions.cs" company="Hoff">
// Copyright (c) Company. All rights reserved.
// </copyright>

namespace Gems.TestInfrastructure.RestTest;

public class QAOptions
{
    public string Path { get; set; }

    public bool Recursive { get; set; }

    public List<string> ScopeFileNames { get; set; }

    public HttpClient HttpClient { get; set; }

    public AllureOptions Allure { get; set; }
}
