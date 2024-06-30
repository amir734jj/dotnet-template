using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Dal.Interfaces;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http.Internal;
using Models.ViewModels.Api;

namespace Logic.Logic;

public class ImageUploadLogic(IFileService fileService) : IImageUploadLogic
{
    public async Task<string> Upload(UploadViewModel file)
    {
        // Randomly assign a key!
        var key = Guid.NewGuid().ToString();

        await fileService.Upload(key, file.File.Name, file.File.ContentType, file.File.OpenReadStream(), new Dictionary<string, string>
        {
            ["Description"] = file.Description
        });

        return key;
    }

    public async Task<UploadViewModel> Download(Guid id)
    {
        var response = await fileService.Download(id.ToString());

        var formFile = new FormFile(response.Data, 0, response.Data.Length, id.ToString(), response.Name)
        {
            ContentType = response.ContentType
        };

        return new UploadViewModel
        {
            File = formFile,
            Description = response.MetaData["Description"]
        };
    }

    public async Task<bool> Delete(Guid id)
    {
        var response = await fileService.Delete(id.ToString());

        return response.Status != HttpStatusCode.BadRequest;
    }
}