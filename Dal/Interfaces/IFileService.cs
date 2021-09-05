using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Models.ViewModels.S3;

namespace Dal.Interfaces
{
    public interface IFileService
    {
        Task<GenericFileServiceResponse> Upload(string fileKey,
            string fileName, string contentType,
            Stream data,
            IDictionary<string, string> metadata);

        Task<DownloadFileServiceResponse> Download(string keyName);
        
        Task<GenericFileServiceResponse> Delete(string keyName);

        Task<List<string>> List();
    }
}