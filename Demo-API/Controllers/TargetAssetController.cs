using Demo_API.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TargetAssetController : ControllerBase
{
    private readonly ITargetAssetBL _targetAssetBL;
    public TargetAssetController(ITargetAssetBL targetAssetBL)
    {
        _targetAssetBL = targetAssetBL;
    }

    [HttpGet]
    public async Task<IActionResult> GetTargetAssets()
    {
        try
        {
            var retVal = await _targetAssetBL.ProcessTargetAssets();
            return Ok(retVal);
        }

        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
