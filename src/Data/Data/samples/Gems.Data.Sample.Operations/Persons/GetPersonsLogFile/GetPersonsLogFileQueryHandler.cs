// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.Sample.Operations.Persons.GetPersonsLogFile.Entities;
using Gems.Data.UnitOfWork;
using Gems.Mvc.GenericControllers;
using Gems.Text.Json;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Gems.Data.Sample.Operations.Persons.GetPersonsLogFile
{
    [Endpoint("api/v1/persons/logs-file", "GET", OperationGroup = "Logs")]
    public class GetPersonsLogFileQueryHandler : IRequestHandler<GetPersonsLogFileQuery, FileStreamResult>
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public GetPersonsLogFileQueryHandler(IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public async Task<FileStreamResult> Handle(GetPersonsLogFileQuery query, CancellationToken cancellationToken)
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
