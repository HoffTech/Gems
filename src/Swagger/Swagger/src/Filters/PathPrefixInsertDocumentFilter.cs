// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Linq;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gems.Swagger.Filters
{
    public class PathPrefixInsertDocumentFilter : IDocumentFilter
    {
        private readonly string pathPrefix;

        public PathPrefixInsertDocumentFilter(string prefix)
        {
            this.pathPrefix = prefix;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Keys.ToList();
            foreach (var path in paths)
            {
                var pathToChange = swaggerDoc.Paths[path];
                swaggerDoc.Paths.Remove(path);
                swaggerDoc.Paths.Add($"{(!string.IsNullOrEmpty(this.pathPrefix) ? "/" : string.Empty)}{this.pathPrefix}{path}", pathToChange);
            }
        }
    }
}
