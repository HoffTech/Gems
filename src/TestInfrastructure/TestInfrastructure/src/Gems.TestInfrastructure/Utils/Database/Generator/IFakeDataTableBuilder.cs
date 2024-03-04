namespace Gems.TestInfrastructure.Utils.Database.Generator
{
    public interface IFakeDataTableBuilder<TRecord> where TRecord : class
    {
        IFakeDataBuilder<TRecord> ForTable(string tableName);
    }
}