// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsLogFile
{
    public class GetPersonsLogFileQuery : IRequest<FileStreamResult>
    {
    }
}
