using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.ViewModels.S3;

namespace Logic.Interfaces
{
    public interface IImageUploadLogic
    {
        Task<string> Upload(IFormFile formFile);

        Task<IFormFile> Download(string id);

        Task<bool> Delete(string id);
    }
}