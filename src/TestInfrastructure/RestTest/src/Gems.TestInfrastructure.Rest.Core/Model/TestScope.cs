namespace Gems.TestInfrastructure.Rest.Core.Model
{
    public class TestScope
    {
        public List<KeyValuePair<string, object>> Variables { get; set; }

        public List<KeyValuePair<string, object>> Templates { get; set; }
    }
}
