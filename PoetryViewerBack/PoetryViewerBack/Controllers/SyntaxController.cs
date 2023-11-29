using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoetryViewerBack.Models;

namespace PoetryViewerBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SyntaxController : ControllerBase
{
    private const int minMinLen = 1;
    private const int maxMinLen = 10;

    [HttpGet("{author}/{minLen}")]
    public string GetIncludes(string author, int minLen)
    {
        Syntax syntax = DTO.Syntax.GetIncludes(author, minLen);
        return JsonConvert.SerializeObject(syntax);
    }


    [HttpGet("random/{author}/{minLen}/{count}")]
    public IActionResult GetWordLocal(string author, int minLen, int count)
    {
        if (minLen > maxMinLen || minLen < minMinLen)
            return BadRequest("Error, out of minLen bounds");
        List<string> res = DTO.Syntax.GetWordsLocal(author, minLen, count);
        return Ok(JsonConvert.SerializeObject(res));
    }


    [HttpGet("random/{minLen}/{count}")]
    public async Task<IActionResult> GetWordGlobal(int minLen, int count)
    {
        if (minLen > maxMinLen || minLen < minMinLen)
            return BadRequest("Error, out of minLen bounds");
        List<string> res = await DTO.Syntax.GetWordsGlobal(minLen, count);
        return Ok(JsonConvert.SerializeObject(res));
    }
}
