using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetLogFile
{
    public class GetLogFileQuery : IRequest<FileStreamResult>
    {
    }
}
