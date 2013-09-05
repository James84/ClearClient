using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ClearClient
{
    public static class Extns
    {
        public static string ToJson(this object content)
        {
            return JsonConvert.SerializeObject(content);
        }


        public static StringContent Serialize(this object content)
        {
            return new StringContent(content.ToJson(), Encoding.UTF8, "application/json");
        }
    }
}
