using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoleEnum
    {
        User = 0,
        Admin = 1
    }
}