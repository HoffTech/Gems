// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

using MediatR;

namespace Gems.Metrics.LabelsProvider
{
    public class LabelsProviderSelector
    {
        private readonly IEnumerable<ILabelsProvider> labelsProviders;

        public LabelsProviderSelector(IEnumerable<ILabelsProvider> labelsProviders)
        {
            this.labelsProviders = labelsProviders;
        }

        public ILabelsProvider<TRequest> GetLabelsProvider<TRequest>()
            where TRequest : IBaseRequest
        {
            return this.labelsProviders.OfType<ILabelsProvider<TRequest>>().FirstOrDefault();
        }
    }
}
