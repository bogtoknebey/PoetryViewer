namespace PoetryViewerBack.Models
{
    public class CreatePoetryResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }

        public CreatePoetryResponse(bool Error, string Message = "Default message", string Name = "Default name")
        {
            this.Error = Error;
            this.Message = Message;
            this.Name = Name;
        }
    }
}
