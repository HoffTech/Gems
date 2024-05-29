// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System;

using Gems.Metrics.Behaviors;
using Gems.Metrics.Samples.Behaviors.TimeMetricBehavior.Custom.Persons.CreatePerson.Dto;

using MediatR;

namespace Gems.Metrics.Samples.Behaviors.TimeMetricBehavior.Custom.Persons.CreatePerson
{
    public record CreatePersonCommand : IRequest<PersonDto>, IRequestTimeMetric
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public int Age { get; init; }

        public Gender Gender { get; init; }

        public Enum GetTimeMetricType()
        {
            return CreatePersonMetricType.CreatePersonTime;
        }
    }
}
