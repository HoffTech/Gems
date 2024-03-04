using System.Data;

namespace Gems.TestInfrastructure.Utils.Database
{
    public static class DataRowExtensions
    {
        public static T? GetValue<T>(this DataRow row, string columnName)
            where T : struct
        {
            var value = row[columnName];
            return value == DBNull.Value ? null : (T?)value;
        }

        public static T GetValue<T>(this DataRow row, string columnName, T defaultValue = default)
            where T : class
        {
            var value = row[columnName];
            return value == DBNull.Value ? defaultValue : (T)value;
        }
    }
}
