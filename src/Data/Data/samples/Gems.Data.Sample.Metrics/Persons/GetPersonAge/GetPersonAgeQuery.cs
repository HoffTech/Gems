// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Metrics.Persons.GetPersonAge
{
    public class GetPersonAgeQuery : IRequest<int>
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
