namespace PoetryViewerBack.Models;

public class AudioRecordByte
{
    public string Author { get; set; }
    public int PoetryNum { get; set; }
    public byte[] AudioData { get; set; }
}
