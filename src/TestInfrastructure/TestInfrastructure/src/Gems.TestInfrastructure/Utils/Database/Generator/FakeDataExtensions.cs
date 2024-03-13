using System.Data.Common;

namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    public static class FakeDataExtensions
    {
        public static IFakeDataTableBuilder<TRecord> FakeData<TRecord>(
            this DbConnection connection,
            Func<TRecord> generator)
            where TRecord : class
        {
            return new FakeDataBuilder<TRecord>(connection, generator);
        }
    }
}
