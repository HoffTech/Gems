// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;

using Gems.Metrics.Samples.Labels.Persons.CreatePerson.Dto;

using MediatR;

namespace Gems.Metrics.Samples.Labels.Persons.ImportPersons
{
    public record ImportPersonsCommand : IRequest
    {
        public List<PersonDto> Persons { get; set; }
    }
}
