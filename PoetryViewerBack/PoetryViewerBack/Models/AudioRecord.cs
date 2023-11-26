using System.Reflection.Metadata;

namespace PoetryViewerBack.Models
{
    public class AudioRecord
    {
        public string Author { get; set; }
        public int PoetryNum { get; set; }
        public IFormFile AudioData { get; set; }
    }
}
