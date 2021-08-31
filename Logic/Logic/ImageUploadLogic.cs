using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Dal.Interfaces;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Logic.Logic
{
    public class ImageUploadLogic : IImageUploadLogic
    {
        private readonly IFileService _fileService;

        public ImageUploadLogic(IFileService fileService)
        {
            _fileService = fileService;
        }
        
        public async Task<string> Upload(IFormFile formFile)
        {
            // Randomly assign a key!
            var key = Guid.NewGuid().ToString();

            await _fileService.Upload(key, formFile.FileName, formFile.ContentType, formFile.OpenReadStream(), new Dictionary<string, string>());

            return key;
        }

        public async Task<IFormFile> Download(string id)
        {
            var response = await _fileService.Download(id);

            var formFile = new FormFile(response.Data, 0, response.Data.Length, id, response.Name)
            {
                ContentType = response.ContentType
            };

            return formFile;
        }

        public async Task<bool> Delete(string keyName)
        {
            var response = await _fileService.Delete(keyName);

            return response.Status != HttpStatusCode.BadRequest;
        }
    }
}