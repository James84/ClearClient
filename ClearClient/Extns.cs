using Newtonsoft.Json;

namespace ClearClient
{
    public static class Extns
    {
        public static string ToJson(this object content)
        {
            return JsonConvert.SerializeObject(content);
        }
    }
}
