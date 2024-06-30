using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoleEnum
{
    Tenant = 0,
    Landlord = 2,
    Admin = 4
}