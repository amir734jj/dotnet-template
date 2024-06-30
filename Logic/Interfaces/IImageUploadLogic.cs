using System;
using System.Threading.Tasks;
using Models.ViewModels.Api;

namespace Logic.Interfaces;

public interface IImageUploadLogic
{
    Task<string> Upload(UploadViewModel file);

    Task<UploadViewModel> Download(Guid id);

    Task<bool> Delete(Guid id);
}