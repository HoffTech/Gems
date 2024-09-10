using System.Dynamic;
using System.Globalization;
using System.Text.RegularExpressions;

using CodingSeb.ExpressionEvaluator;

using Gems.TestInfrastructure.Rest.Core.Asserts;
using Gems.TestInfrastructure.Rest.Core.Library;
using Gems.TestInfrastructure.Rest.Core.Templates;

using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.Rest.Core;

public class TestRunnerContext
{
    private readonly Regex rxTimeSpan = new Regex(@"^(?<n>\d+)(?<u>h|m|s|ms)$");
    private readonly ExpressionEvaluator evaluator;
    private readonly Stack<Dictionary<string, object>> scopeVariables;

    public TestRunnerContext(TestRunnerContextOptions options = null)
    {
        if (options == null)
        {
            options = new TestRunnerContextOptions();
        }

        this.evaluator = new ExpressionEvaluator();
        options.Namespaces?.ForEach(ns => this.evaluator.Namespaces.Add(ns));
        this.evaluator.Variables.Add("Fake", new FakerLibrary(options.Faker?.Locale ?? "en"));
        this.evaluator.Variables.Add("Assert", new AssertLibrary());
        this.evaluator.Variables.Add("Is", new AssertIsLibrary());
        this.scopeVariables = new Stack<Dictionary<string, object>>();
    }

    public void SetVariable(string name, object value)
    {
        if (this.evaluator.Variables.ContainsKey(name))
        {
            this.evaluator.Variables.Remove(name);
        }

        this.evaluator.Variables.Add(name, this.Eval(value));
    }

    public void SetTemplateVariable(string name, object value)
    {
        if (this.evaluator.Variables.ContainsKey(name))
        {
            this.evaluator.Variables.Remove(name);
        }

        this.evaluator.Variables.Add(name, new Template(this, value));
    }

    public void PushScope()
    {
        this.scopeVariables.Push(new Dictionary<string, object>(this.evaluator.Variables));
    }

    public void PopScope()
    {
        this.evaluator.Variables.Clear();
        if (this.scopeVariables.Count > 0)
        {
            this.evaluator.Variables = this.scopeVariables.Pop();
        }
    }

    public object Eval(string expression)
    {
        var segments = Mustache.Parse(expression).ToList();
        if (segments.Count == 0)
        {
            return string.Empty;
        }
        else if (segments.Count == 1)
        {
            var first = segments.First();
            return first.SegmentType switch
            {
                MustacheSegmentType.Text => first.Value,
                MustacheSegmentType.Expression => this.EvalExpression(first.Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            return string.Join(string.Empty, segments.Select(s => s.SegmentType switch
            {
                MustacheSegmentType.Text => s.Value,
                MustacheSegmentType.Expression => this.EvalExpression(s.Value)?.ToString() ?? string.Empty,
                _ => throw new ArgumentOutOfRangeException()
            }));
        }
    }

    public object EvalExpression(string expression)
    {
        var result = this.evaluator.Evaluate(expression);
        if (result != null && result is Template template)
        {
            return template.RenderedValue;
        }

        return result;
    }

    public TimeSpan? ParseTimeSpan(string expression)
    {
        return this.TryParseTimeSpan(expression, out var ts) ? ts : null;
    }

    public bool TryParseTimeSpan(string expression, out TimeSpan value)
    {
        value = TimeSpan.Zero;
        if (string.IsNullOrEmpty(expression))
        {
            return false;
        }

        if (expression.Equals("0"))
        {
            return true;
        }

        if (expression.Equals("inf"))
        {
            value = TimeSpan.MaxValue;
            return true;
        }

        if (TimeSpan.TryParse(expression, out value))
        {
            return true;
        }

        var m = this.rxTimeSpan.Match(expression);
        if (m.Success)
        {
            var n = m.Groups["n"];
            var u = m.Groups["u"];
            if (n.Success && u.Success)
            {
                var nValue = int.Parse(n.Value);
                value = u.Value switch
                {
                    "h" => TimeSpan.FromHours(nValue),
                    "m" => TimeSpan.FromMinutes(nValue),
                    "s" => TimeSpan.FromSeconds(nValue),
                    "ms" => TimeSpan.FromMilliseconds(nValue),
                    _ => throw new ArgumentOutOfRangeException(nameof(expression)),
                };

                return true;
            }
        }

        return false;
    }

    public object Eval(object source)
    {
        if (source == null)
        {
            return null;
        }

        if (source is string s)
        {
            return this.Eval(s);
        }

        if (source is JObject jObject)
        {
            return this.EvalJObject(jObject);
        }

        if (source is JArray jArray)
        {
            return this.EvalJArray(jArray);
        }

        if (source is JValue jValue)
        {
            return this.EvalJValue(jValue);
        }

        return source;
    }

    private object EvalJValue(JValue jValue)
    {
        return this.Eval(jValue.Value);
    }

    private object EvalJArray(JArray jArray)
    {
        return jArray
            .Children()
            .Select(child => this.Eval(child))
            .ToList();
    }

    private object EvalJObject(JObject jObject)
    {
        var expandoObject = new ExpandoObject();
        foreach (var item in jObject.Children())
        {
            if (item is JProperty jProperty)
            {
                expandoObject.TryAdd(jProperty.Name, this.Eval(jProperty.Value));
            }
        }

        return expandoObject;
    }
}
