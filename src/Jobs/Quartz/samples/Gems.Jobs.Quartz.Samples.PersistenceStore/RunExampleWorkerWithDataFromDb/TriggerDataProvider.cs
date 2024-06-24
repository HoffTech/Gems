// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gems.Data.UnitOfWork;
using Gems.Jobs.Quartz.Jobs.JobTriggerFromDb;
using Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithDataFromDb.Entities;

namespace Gems.Jobs.Quartz.Samples.PersistenceStore.RunExampleWorkerWithDataFromDb;

public class TriggerDataProvider : ITriggerDataProvider
{
    private readonly IUnitOfWorkProvider unitOfWorkProvider;

    public TriggerDataProvider(IUnitOfWorkProvider unitOfWorkProvider)
    {
        this.unitOfWorkProvider = unitOfWorkProvider;
    }

    public async Task<Dictionary<string, object>> GetTriggerData(string triggerName, CancellationToken cancellationToken)
    {
        var data = await this.unitOfWorkProvider.GetUnitOfWork(cancellationToken)
            .CallTableFunctionFirstAsync<SomeData>(
                "quartz.get_some_data",
                new Dictionary<string, object> { { "p_trigger_name", triggerName }, });

        return new Dictionary<string, object> { { "Id", data.Id }, { "Data", data.Data } };
    }
}
