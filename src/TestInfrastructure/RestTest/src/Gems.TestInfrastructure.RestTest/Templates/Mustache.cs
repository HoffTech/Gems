using System.Text;

namespace Gems.TestInfrastructure.RestTest.Templates;

public class Mustache
{
    public static IEnumerable<MustacheSegment> Parse(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            var buffer = new StringBuilder();
            var i = 0;
            while (true)
            {
                if (Eos(text, i))
                {
                    break;
                }

                ReadText(text, ref i, buffer);
                if (buffer.Length > 0)
                {
                    yield return new MustacheSegment()
                    {
                        SegmentType = MustacheSegmentType.Text,
                        Value = buffer.ToString(),
                    };
                    buffer.Clear();
                }

                if (Eos(text, i))
                {
                    break;
                }

                ReadExpression(text, ref i, buffer);
                if (buffer.Length > 0)
                {
                    yield return new MustacheSegment()
                    {
                        SegmentType = MustacheSegmentType.Expression,
                        Value = buffer.ToString(),
                    };
                    buffer.Clear();
                }
            }
        }
    }

    private static bool Eos(string text, int i)
    {
        return i >= text.Length;
    }

    private static void ReadText(string text, ref int i, StringBuilder buffer)
    {
        while (!Eos(text, i) && !StartOfExpression(text, i))
        {
            buffer.Append(text[i]);
            i++;
        }

        if (!Eos(text, i))
        {
            i += 2;
        }
    }

    private static void ReadExpression(string text, ref int i, StringBuilder buffer)
    {
        while (!Eos(text, i) && !EndOfExpression(text, i))
        {
            buffer.Append(text[i]);
            i++;
        }

        if (Eos(text, i))
        {
            throw new ArgumentException("Unexpected end of string");
        }

        i += 2;
    }

    private static bool StartOfExpression(string text, int i)
    {
        return (i + 1) < text.Length && text.Substring(i, 2).Equals("{{");
    }

    private static bool EndOfExpression(string text, int i)
    {
        return (i + 1) < text.Length && text.Substring(i, 2).Equals("}}");
    }
}
