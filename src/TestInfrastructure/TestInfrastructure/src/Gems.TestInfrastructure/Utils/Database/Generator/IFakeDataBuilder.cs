// Licensed to the Hoff Tech under one or more agreements.
// The Hoff Tech licenses this file to you under the MIT license.

namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    public interface IFakeDataBuilder<TRecord> where TRecord : class
    {
        Task BuildAsync(CancellationToken cancellationToken = default);

        IFakeDataBuilder<TRecord> WithCount(int count);

        IFakeDataBuilder<TRecord> WithCount(int minCount, int maxCount);
    }
}
