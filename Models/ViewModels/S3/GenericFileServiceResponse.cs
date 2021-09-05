using System.Net;

namespace Models.ViewModels.S3
{
    public class GenericFileServiceResponse
    {
        public HttpStatusCode Status { get; }
        
        public string Message { get; }

        public GenericFileServiceResponse(HttpStatusCode status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}