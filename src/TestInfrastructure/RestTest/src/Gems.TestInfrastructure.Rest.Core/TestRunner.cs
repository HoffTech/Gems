using System.Diagnostics;
using System.Dynamic;
using System.Net.Mime;
using System.Text;

using Allure.Net.Commons;

using Gems.TestInfrastructure.Rest.Core.Asserts;
using Gems.TestInfrastructure.Rest.Core.Builders;
using Gems.TestInfrastructure.Rest.Core.Model;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunner : IDisposable
{
    private readonly TestRunnerContext context;
    private readonly AssertionManager assertionManager;
    private readonly HttpClient externalHttpClient;
    private readonly ILogger<TestRunner> logger;
    private HttpClient httpClient;

    public TestRunner(
        TestRunnerOptions options = null,
        ILogger<TestRunner> logger = null)
    {
        if (options == null)
        {
            options = new TestRunnerOptions();
        }

        this.externalHttpClient = options.HttpClient;
        this.context = new TestRunnerContext(options.Context);
        this.assertionManager = new AssertionManager(this.context);
        this.logger = logger;
    }

    public async Task<bool> RunAsync(
        TestCollection collection,
        IEnumerable<TestScope> globals,
        CancellationToken cancellationToken = default)
    {
        var lifeCycle = AllureLifecycle.Instance;
        lifeCycle.StartTestContainer(CreateAllureTestResultContainer(collection));
        AllureContext context = null;
        try
        {
            var success = true;
            foreach (var scope in globals)
            {
                this.MergeScope(scope);
            }

            this.MergeScope(collection);

            if (collection.Tests != null)
            {
                var outputs = new List<KeyValuePair<string, object>>();

                foreach (var test in collection.Tests)
                {
                    test.Variables.AddRange(outputs);

                    success &= this.InternalRunTestAsync(test, cancellationToken)
                        .GetAwaiter()
                        .GetResult();

                    outputs.AddRange(test.Output);
                }
            }

            return success;
        }
        finally
        {
            lifeCycle.WriteTestContainer();
        }
    }

    public void Dispose()
    {
        this.httpClient?.Dispose();
    }

    private HttpClient GetOrCreateHttpClient()
    {
        if (this.externalHttpClient != null)
        {
            return this.externalHttpClient;
        }

        if (this.httpClient == null)
        {
            this.httpClient = new HttpClient();
        }

        return this.httpClient;
    }

    private static object SafeDeserializeJson(string text)
    {
        try
        {
            return JsonConvert.DeserializeObject(text);
        }
        catch
        {
            return new ExpandoObject();
        }
    }

    private static TestResultContainer CreateAllureTestResultContainer(TestCollection collection)
    {
        var qaContainer = new TestResultContainer()
        {
            name = collection.Name ?? "Test Collection",
            uuid = Guid.NewGuid().ToString("N"),
            description = collection.Description,
        };

        return qaContainer;
    }

    private static void WriteTrace(StringBuilder trace, string message, Exception e = null)
    {
        trace.AppendLine(message);
        if (e != null)
        {
            while (e != null)
            {
                trace.AppendLine($"{e} ({e.Source}): {e.Message}");
                trace.AppendLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    trace.AppendLine("=== Inner Exception ===");
                }

                e = e.InnerException;
            }

            trace.AppendLine();
        }
    }

    private async Task<bool> InternalRunTestAsync(
        Test test,
        CancellationToken cancellationToken)
    {
        var lifeCycle = AllureLifecycle.Instance;
        lifeCycle.StartTestCase(new()
        {
            name = test.Name,
            description = test.Description,
            uuid = Guid.NewGuid().ToString("N"),
        });

        var success = false;
        var trace = new StringBuilder();
        this.logger?.LogInformation(
            "Test started: {Name}, {Description}, {Author}",
            test.Name,
            test.Description,
            test.Author);
        this.PushScope(test);
        try
        {
            if (test.Request != null)
            {
                var builder = new HttpRequestDefinitionBuilder(this.context);
                await this.ExecuteRequestAsync(builder.Build(test.Request), cancellationToken);
            }

            this.EvalOutput(test);
            success = true;

            foreach (var assertItem in test.Asserts)
            {
                success &= this.Assert(assertItem);
            }
        }
        catch (Exception e)
        {
            this.logger?.LogError(e, "Test failed");
            WriteTrace(trace, $"Test failed", e);
            success = false;
        }
        finally
        {
            this.PopScope();
            lifeCycle.UpdateTestCase(x =>
            {
                x.status = success ? Status.passed : Status.failed;
                x.statusDetails = new StatusDetails()
                {
                    message = success ? "Test failed" : "Test succeeded",
                    trace = trace.ToString(),
                };
            });
            lifeCycle.WriteTestCase();
        }

        this.logger?.LogInformation("Test completed: {Name}", test.Name);
        return success;
    }

    private bool Assert(object assertItem)
    {
        var success = false;
        var lifeCycle = AllureLifecycle.Instance;
        var stepResult = new StepResult();
        var trace = new StringBuilder();
        var lastExceptionMessage = string.Empty;
        stepResult.name = $"Assert: {assertItem}";
        stepResult.status = Status.none;
        lifeCycle.StartStep(stepResult);
        try
        {
            this.assertionManager.Assert(assertItem);
            this.logger?.LogInformation("Assert succeeded: {assert}", assertItem);
            success = true;
        }
        catch (Exception e)
        {
            this.logger?.LogError(e, "Assert failed: {assert}", assertItem);
            WriteTrace(trace, $"Assert failed", e);
            lastExceptionMessage = e.Message;
        }
        finally
        {
            lifeCycle.StopStep(x =>
            {
                x.status = success ? Status.passed : Status.failed;
                x.statusDetails = new StatusDetails() { trace = trace.ToString(), message = lastExceptionMessage, };
            });
        }

        return success;
    }

    private async Task ExecuteRequestAsync(
        HttpRequestDefinition httpRequest,
        CancellationToken cancellationToken)
    {
        var success = false;
        var lifeCycle = AllureLifecycle.Instance;
        var stepResult = new StepResult();
        stepResult.name = $"{httpRequest.Method} {httpRequest.Address}";
        stepResult.status = Status.none;

        this.logger?.LogInformation(
            "Request started: {Method} {Address}",
            httpRequest.Method,
            httpRequest.Address.ToString());
        var client = this.GetOrCreateHttpClient();
        using var message = new HttpRequestMessage();
        message.RequestUri = httpRequest.Address;
        message.Method = httpRequest.Method;

        if (httpRequest.Body != null)
        {
            var jsonBody = JsonConvert.SerializeObject(httpRequest.Body);
            this.logger?.LogInformation("Request body:\n{Body}", jsonBody);
            message.Content = new StringContent(
                jsonBody,
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
            stepResult.parameters.Add(new Parameter()
            {
                name = "Request.Body",
                value = jsonBody,
            });
        }

        if (httpRequest.Headers != null)
        {
            foreach (var header in httpRequest.Headers)
            {
                this.logger?.LogInformation("Request header: {Name}={Value}", header.Key, header.Value);
                message.Headers.Add(header.Key, header.Value);
                stepResult.parameters.Add(new Parameter()
                {
                    name = $"Headers.{header.Key}",
                    value = header.Value,
                });
            }
        }

        lifeCycle.StartStep(stepResult);
        try
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using var response = await client.SendAsync(message, cancellationToken);
            stopWatch.Stop();
            this.logger?.LogInformation(
                "Request completed in {Elapsed}ms with {StatusCode}",
                stopWatch.ElapsedMilliseconds,
                (int)response.StatusCode);
            var responseVariable = new ExpandoObject();
            foreach (var header in response.Headers)
            {
                this.logger?.LogInformation("Response header: {Name}={Value}", header.Key, header.Value);
            }

            var bodyAsString = await response.Content.ReadAsStringAsync(cancellationToken);

            this.logger?.LogInformation("Response body:\n{Body}", bodyAsString);

            var body = new ExpandoObject();
            body.TryAdd("Text", bodyAsString);
            body.TryAdd("Json", SafeDeserializeJson(bodyAsString));

            responseVariable.TryAdd("Body", body);
            responseVariable.TryAdd("StatusCode", (int)response.StatusCode);
            responseVariable.TryAdd("Headers", response.Headers.ToDictionary(x => x.Key, x => x.Value));
            this.context.SetVariable("Response", responseVariable);
            success = true;
        }
        finally
        {
            lifeCycle.StopStep(x => x.status = success ? Status.passed : Status.failed);
        }
    }

    private void EvalOutput(Test test)
    {
        if (test.Output is null)
        {
            return;
        }

        var tmp = new List<KeyValuePair<string, object>>();

        foreach (var kv in test.Output)
        {
            tmp.Add(new KeyValuePair<string, object>(kv.Key, this.context.Eval(kv.Value)));

            // похоже, это здесь не нужно, оно теряется при PopScope() внутри InternalRunTestAsync блока finally
            // this.context.SetVariable(kv.Key, tmp);
        }

        test.Output = tmp;
    }

    private void MergeScope(TestScope scope)
    {
        scope.Variables?.ForEach(kv => this.context.SetVariable(kv.Key, kv.Value));
        scope.Templates?.ForEach(kv => this.context.SetTemplateVariable(kv.Key, kv.Value));
    }

    private void PushScope(TestScope scope)
    {
        this.context.PushScope();
        this.MergeScope(scope);
    }

    private void PopScope()
    {
        this.context.PopScope();
    }
}
