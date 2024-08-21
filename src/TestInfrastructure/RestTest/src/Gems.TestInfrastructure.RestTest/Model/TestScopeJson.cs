using Newtonsoft.Json.Linq;

namespace Gems.TestInfrastructure.RestTest.Model
{
    public class TestScopeJson
    {
        public JObject Variables { get; set; }

        public JObject Templates { get; set; }
    }
}
