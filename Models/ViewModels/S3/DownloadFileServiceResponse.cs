using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Models.ViewModels.S3
{
    public class DownloadFileServiceResponse : GenericFileServiceResponse
    {
        public MemoryStream Data { get; }
        
        public IReadOnlyDictionary<string, string> MetaData { get; }
        
        public string ContentType { get; }
        
        public string Name { get; }

        public DownloadFileServiceResponse(HttpStatusCode statusCode, string message) : base(statusCode, message)
        {
        }

        public DownloadFileServiceResponse(HttpStatusCode status, string message, MemoryStream data, IReadOnlyDictionary<string, string> metaData, string contentType, string name) : base(status, message)
        {
            Data = data;
            MetaData = metaData;
            ContentType = contentType;
            Name = name;
        }
    }
}