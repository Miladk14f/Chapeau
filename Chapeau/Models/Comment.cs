namespace Chapeau.Models
{
    public class Comment
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
