using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dal.Interfaces;
using LiteDB;
using Models.ViewModels.S3;

namespace Dal.ServiceApi
{
    public class LiteDbFileService : IFileService
    {
        private readonly ILiteStorage<string> _storage;

        public LiteDbFileService(ILiteDatabase database)
        {
            _storage = database.FileStorage;
        }

        public async Task<GenericFileServiceResponse> Upload(string fileKey, string fileName, string contentType,
            Stream data, IDictionary<string, string> metadata)
        {
            metadata["Name"] = fileName;
            metadata["Content-Type"] = contentType;
            
            _storage.Upload(fileKey, fileName, data,
                new BsonDocument(
                    new Dictionary<string, BsonValue>(metadata.ToDictionary(x => x.Key, x => new BsonValue(x.Value)))));

            return new GenericFileServiceResponse(
                HttpStatusCode.Accepted,
                "Successfully uploaded file to LiteDb storage"
            );
        }

        public async Task<DownloadFileServiceResponse> Download(string keyName)
        {
            var result = new MemoryStream();
            var response = _storage.Download(keyName, result);
            
            var fileName = response.Metadata["Name"];
            var contentType = response.Metadata["Content-Type"];

            return new DownloadFileServiceResponse(
                HttpStatusCode.OK,
                "Successfully downloaded uploaded file from LiteDb storage",
                result,
                new ReadOnlyDictionary<string, string>(response.Metadata.ToDictionary(x => x.Key, x => x.Value.AsString)),
                contentType, fileName);
        }

        public async Task<List<string>> List()
        {
            return _storage.FindAll().Select(x => x.Id).ToList();
        }
    }
}