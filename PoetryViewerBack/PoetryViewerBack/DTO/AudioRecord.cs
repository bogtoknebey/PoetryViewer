namespace PoetryViewerBack.DTO;

using Microsoft.VisualBasic;
using PoetryViewerBack.Models;
using NAudio.Wave;
public static class AudioRecord
{
    public static bool CreateWavFile(string filePath, byte[] audioData, int sampleRate = 44100, int channels = 2, int bitDepth = 16)
    {
        try
        {
            using (WaveFileWriter waveFileWriter = new WaveFileWriter(filePath, new WaveFormat(sampleRate, bitDepth, channels)))
            {
                waveFileWriter.Write(audioData, 0, audioData.Length);
            }
            return true;
        }
        catch { return false; }
    }
    public async static Task<List<AudioDataResponse>?> GetAudio(string author, string poetryNum, int pageCapacity, int pageNumber)
    {
        string path = $"data/{author}/{poetryNum}/audio";
        if (!Directory.Exists(path)) return null;

        int from = pageCapacity * (pageNumber - 1);
        int to = pageCapacity * pageNumber - 1;
        string[] audioFiles = Directory.GetFiles(path, "*.wav");

        if (from > audioFiles.Length - 1) return null;
        if (to > audioFiles.Length - 1) 
            to = audioFiles.Length - 1;
        if (from > to) return null;

        List<AudioDataResponse> res = new List<AudioDataResponse>();
        AudioDataResponse rec = null;
        try
        {
            for (int i = from; i <= to; i++)
            {
                rec = new AudioDataResponse()
                {
                    Name = audioFiles[i].Split('\\', '/').Last(),
                    AudioByteData = File.ReadAllBytes(audioFiles[i])
                };
                res.Add(rec);
            }
        } 
        catch { return null; }

        return res;
    }
    public async static Task<bool> CreateAudio(Models.AudioRecord rec)
    {
        string path = $"data/{rec.Author}/{rec.PoetryNum}/audio";
        path = DTO.Poetry.GetNextFilePath(path, "wav", "file");

        if (rec.AudioData.Length == 0)
            return false;

        try
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await rec.AudioData.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
    public async static Task<bool> CreateAudio(Models.AudioRecordByte rec)
    {
        string path = $"data/{rec.Author}/{rec.PoetryNum}/audio";
        path = DTO.Poetry.GetNextFilePath(path, "wav", "file");

        if (rec.AudioData.Length == 0)
            return false;

        try
        {
            File.WriteAllBytes(path, rec.AudioData);
        }
        catch
        {
            return false;
        }

        return true;
        //return CreateWavFile(path, rec.AudioData);
    }

    public async static Task<bool> DeleteAudio(string author, string poetryName, string audioName)
    {
        string filePath = $"data/{author}/{poetryName}/audio/{audioName}";
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
        } catch { return false; }
        return false;
    }
}
