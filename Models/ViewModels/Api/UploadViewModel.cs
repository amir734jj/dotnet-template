using Microsoft.AspNetCore.Http;

namespace Models.ViewModels.Api
{
    public class UploadViewModel
    {
        public string Description { get; set; }

        public IFormFile File { get; set; }
    }
}