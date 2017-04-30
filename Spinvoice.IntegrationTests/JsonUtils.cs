using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Spinvoice.IntegrationTests
{
    public class JsonUtils
    {
        public static T Deserialize<T>(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture
            });
            return result;
        }
    }
}