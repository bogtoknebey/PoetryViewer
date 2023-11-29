using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using PoetryViewerBack.DTO;
using PoetryViewerBack.Models;
using PoetryViewerBack.External.Translator;

namespace PoetryViewerBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PoetryController : ControllerBase
{
    [HttpGet]
    public string GetAuthorsList()
    {
        return JsonConvert.SerializeObject(DTO.Poetry.GetAuthorsList());
    }


    [HttpGet("random/{author}")]
    public string GetRandomPoetry(string author)
    {
        return JsonConvert.SerializeObject(DTO.Poetry.GetRandomPoetry(author));
    }


    [HttpGet("count/{author}")]
    public string GetPoetryCount(string author)
    {
        int lastNum = DTO.Poetry.GetPoetryCount(author);
        return JsonConvert.SerializeObject(lastNum);
    }


    [HttpGet("{author}/{id}")]
    public string Index(string author, int id)
    {
        return JsonConvert.SerializeObject(DTO.Poetry.GetPoetry(author, id));
    }


    [HttpPost("translate")]
    public string TranslateOperation([FromBody]Models.Translate t)
    {
        string res = Translator.GetTranslate(t.Text, t.SwitchTimes);
        return JsonConvert.SerializeObject(res);
    }


    [HttpPost("create")]
    public IActionResult CreatePoetry([FromBody] Models.Poetry p)
    {
        if (p is null || p.Text is null || p.Text == "")
            return BadRequest(new { Message = "Failed to add, empty text." });

        CreatePoetryResponse response = DTO.Poetry.CreatePoetry(p.Author, p.Text);

        if (response.Error)
            return BadRequest(response);
        return Ok(response);                
    }


    [HttpPost("put")]
    public IActionResult UpdatePoetry([FromBody] Models.Poetry p)
    {
        var successResp = new { Message = "File updated successfully." };
        var unSuccessResp = new { Message = "Updating Error." };
        bool res = DTO.Poetry.UpdatePoetry(p);
        if (res)
            return Ok(successResp);
        return BadRequest(unSuccessResp);
    }


    [HttpPost("delete")]
    public IActionResult DeletePoetry([FromBody] Models.Poetry p)
    {
        var successResp = new { Message = "File deleted successfully." };
        var unSuccessResp = new { Message = "Deleting Error." };
        bool res = DTO.Poetry.DeletePoetry(p);
        if (res)
            return Ok(successResp);
        return BadRequest(unSuccessResp);
    }
}
