﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PoetryViewerBack.DTO;
using PoetryViewerBack.Models;
using PoetryViewerBack.External.Audiotor;
using System.Reflection.Metadata;


namespace PoetryViewerBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoiceController : ControllerBase
{
    [HttpGet("{author}/{poetryNum}/{pageCapacity}/{pageNumber}")]
    public async Task<IActionResult> GetAudio(string author, string poetryNum, int pageCapacity, int pageNumber)
    {
        // /api/voice/I/1/1/1
        List<Models.AudioDataResponse>? res = await DTO.AudioRecord.GetAudio(author, poetryNum, pageCapacity, pageNumber);
        if (res is null)
            return BadRequest("Something went wrong.");
        return Ok(JsonConvert.SerializeObject(res));
    }


    [HttpPost("autocreate")]
    public async Task<IActionResult> AutoCreateAudio([FromBody] Models.Poetry p)
    {
        var successResp = new { Message = "Audio uploaded successfully." };
        var unSuccessStroreResp = new { Message = "Store Recording Error." };

        AudioRecordByte? rec = Auditor2.GetAudioByPoetry(p);
        if (rec is null || rec.PoetryNum == -1)
        {
            var unSuccessCreationResp = new { Message = $"RESPONSE DATA: Author:{rec?.Author} PoetryNum: {rec?.PoetryNum} AudioLength{rec?.AudioData.Length}" };
            return BadRequest(unSuccessCreationResp);
        }

        bool res = await DTO.AudioRecord.CreateAudio(rec);
        if (!res)
            return BadRequest(unSuccessStroreResp);
        return Ok(successResp);
    }


    [HttpPost]
    public async Task<IActionResult> UploadAudio([FromForm] Models.AudioRecord rec)
    {
        bool res = await DTO.AudioRecord.CreateAudio(rec);
        var successResp = new { Message = "Audio uploaded successfully." };
        var unSuccessResp = new { Message = "Store Recording Error." };
        if (!res)
            return BadRequest(unSuccessResp);
        return Ok(successResp);
    }


    [HttpPost("delete")]
    public async Task<IActionResult> DeleteAudio([FromForm] Models.DeleteAudio data)
    {
        var successResp = new { Message = "Delete was completed seccessful." };
        var unSuccessResp = new { Message = "Error: Delete was not completed seccessful." };
        bool res = await DTO.AudioRecord.DeleteAudio(data.author, data.poetryName, data.audioName);
        if (!res)
            return BadRequest(JsonConvert.SerializeObject(unSuccessResp));
        return Ok(JsonConvert.SerializeObject(successResp));    
    }
}
