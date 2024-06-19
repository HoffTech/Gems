// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Gems.Mvc;

public class ContentTypeMiddleware
{
    private readonly RequestDelegate next;

    public ContentTypeMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(
            _ =>
            {
                if (context.Response.Headers.TryGetValue("Content-Type", out var originalContentType))
                {
                    var charsetPattern = @"\s*charset=utf-8\s*(;)?\s*";
                    var replacedContentType = Regex.Replace(originalContentType, charsetPattern, string.Empty).Trim(';').Trim();
                    context.Response.Headers.Remove("Content-Type");
                    context.Response.Headers.Append("Content-Type", replacedContentType);
                }

                return Task.CompletedTask;
            }, context);
        await this.next(context);
    }
}
