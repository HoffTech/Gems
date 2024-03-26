using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsLogFile
{
    public class GetPersonsLogFileQuery : IRequest<FileStreamResult>
    {
    }
}
