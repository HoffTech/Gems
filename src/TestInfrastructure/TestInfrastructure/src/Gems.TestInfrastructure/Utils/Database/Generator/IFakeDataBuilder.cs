namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    public interface IFakeDataBuilder<TRecord> where TRecord : class
    {
        Task BuildAsync(CancellationToken cancellationToken = default);

        IFakeDataBuilder<TRecord> WithCount(int count);

        IFakeDataBuilder<TRecord> WithCount(int minCount, int maxCount);
    }
}
