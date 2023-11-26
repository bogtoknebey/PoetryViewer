using System.Buffers.Text;

namespace PoetryViewerBack.Models
{
    public class AudioDataResponse
    {
        public string Name {  get; set; }
        public byte[] AudioByteData { get; set; }
    }
}
