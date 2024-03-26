using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.Sample.Operations.Persons.GetLogFile.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;
using Gems.Text.Json;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetLogFile
{
    [Endpoint("api/v1/logs-file", "GET", OperationGroup = "Logs")]
    public class GetLogFileQueryHandler : IRequestHandler<GetLogFileQuery, FileStreamResult>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public GetLogFileQueryHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task<FileStreamResult> Handle(GetLogFileQuery query, CancellationToken cancellationToken)
        {
            await using (var sw = new StreamWriter("output.txt"))
            {
                await foreach (var log in this.GetLogsAsAsyncEnumerable(cancellationToken))
                {
                    await sw.WriteLineAsync(log.Serialize()).ConfigureAwait(false);
                }
            }

            return new FileStreamResult(File.OpenRead("output.txt"), "text/plain");
        }

        private IAsyncEnumerable<Log> GetLogsAsAsyncEnumerable(CancellationToken cancellationToken)
        {
            return this.unitOfWorkProvider
                .GetUnitOfWork(cancellationToken)
                .ExecuteReaderAsync<Log>("SELECT * FROM public.log");
        }
    }
}
