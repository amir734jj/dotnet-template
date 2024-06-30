using System.Net;

namespace Models.ViewModels.S3;

public class GenericFileServiceResponse(HttpStatusCode status, string message)
{
    public HttpStatusCode Status { get; } = status;

    public string Message { get; } = message;
}