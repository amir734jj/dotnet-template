using System.Net;

namespace Dal.Extensions
{
    public static class HttpStatusCodeExtension
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        }
    }
}