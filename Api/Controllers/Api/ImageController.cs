using System;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Api;

namespace Api.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class ImageController : Controller
    {
        private readonly IImageUploadLogic _imageUploadLogic;

        public ImageController(IImageUploadLogic imageUploadLogic)
        {
            _imageUploadLogic = imageUploadLogic;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> ImageUploadBase64([FromForm] UploadViewModel metadata)
        {
            var response = await _imageUploadLogic.Upload(metadata);

            return Ok(response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> DownloadImage([FromRoute] Guid id)
        {
            var result = await _imageUploadLogic.Download(id);

            return File(result.File.OpenReadStream(), result.File.ContentType, result.File.Name);
        }

        [HttpDelete]
        [Route("{id}/delete")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _imageUploadLogic.Delete(id);

            return Ok(result);
        }
    }
}