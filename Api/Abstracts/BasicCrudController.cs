namespace Api.Abstracts;

using System.Collections;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
public abstract class BasicCrudController<T> : Controller
{
    [NonAction]
    protected abstract IBasicLogic<T> BasicLogic();

    [HttpGet]
    [Route("")]
    [SwaggerOperation("GetAll")]
    [ProducesResponseType(typeof(IEnumerable), 200)]
    public virtual async Task<IActionResult> GetAll()
    {
        return Ok(await BasicLogic().GetAll());
    }

    [HttpGet]
    [Route("{id:int}")]
    [SwaggerOperation("Get")]
    public virtual async Task<IActionResult> Get([FromRoute] int id)
    {
        return Ok(await BasicLogic().Get(id));
    }

    [HttpPut]
    [Route("{id:int}")]
    [SwaggerOperation("Update")]
    public virtual async Task<IActionResult> Update([FromRoute] int id, [FromBody] T instance)
    {
        if (!await AuthorizationGuard(id))
        {
            return BadRequest("Not authorized");
        }

        return Ok(await BasicLogic().Update(id, instance));
    }

    [HttpDelete]
    [Route("{id:int}")]
    [SwaggerOperation("Delete")]
    public virtual async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!await AuthorizationGuard(id))
        {
            return BadRequest("Not authorized");
        }

        return Ok(await BasicLogic().Delete(id));
    }

    [HttpPost]
    [Route("")]
    [SwaggerOperation("Save")]
    public virtual async Task<IActionResult> Save([FromBody] T instance)
    {
        return Ok(await BasicLogic().Save(instance));
    }

    [NonAction]
    protected virtual Task<bool> AuthorizationGuard(int _)
    {
        return Task.FromResult(true);
    }
}