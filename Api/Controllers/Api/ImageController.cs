using System;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Api;

namespace Api.Controllers.Api;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class ImageController(IImageUploadLogic imageUploadLogic) : Controller
{
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadViewModel metadata)
    {
        var response = await imageUploadLogic.Upload(metadata);

        return Ok(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Download([FromRoute] Guid id)
    {
        var result = await imageUploadLogic.Download(id);

        return File(result.File.OpenReadStream(), result.File.ContentType, result.File.Name);
    }

    [HttpDelete]
    [Route("{id:guid}/delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await imageUploadLogic.Delete(id);

        return Ok(result);
    }
}