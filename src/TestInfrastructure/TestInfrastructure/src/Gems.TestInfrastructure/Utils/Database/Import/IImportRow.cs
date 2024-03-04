namespace Gems.TestInfrastructure.Utils.Database.Import
{
    public interface IImportRow
    {
        string[] GetColumns();

        object GetValue(int i);
    }
}
