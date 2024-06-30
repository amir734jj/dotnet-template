namespace Api.Attributes;

using Microsoft.AspNetCore.Mvc;

public class DisallowAuthorizedAttribute() : TypeFilterAttribute(typeof(DisallowAuthorizedFilter));
