// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Mvc.Sample.SourceType.Persons.ImportPerson;

public class ImportPersonCommand : IRequest
{
    [FromForm(Name = "csv_file")]
    public IFormFile CsvFile { get; set; }
}
