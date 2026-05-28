using Microsoft.AspNetCore.Mvc;

namespace RouteOptimizer.Api.Controllers;

[ApiController]
[Route("api/node")]
public sealed class NodeController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetNodeInfo([FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new { error = "Node id is required." });
        }

        return Ok(new
        {
            id,
            message = $"{id} test"
        });
    }
}