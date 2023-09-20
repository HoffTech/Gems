// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using Gems.Mvc.Filters.Errors;

using Microsoft.Extensions.Options;

namespace Gems.Http
{
    public class DefaultClientService : BaseClientService<BusinessErrorViewModel>
    {
        public DefaultClientService(
            IOptions<HttpClientServiceOptions> options,
            BaseClientServiceHelper helper) : base(options, helper)
        {
        }
    }
}
