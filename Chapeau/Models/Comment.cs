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

        public Comment() { }

        public Comment(int id, int orderId, string type, string text, DateTime createdAt)
        {
            Id = id;
            OrderId = orderId;
            Type = type;
            Text = text;
            CreatedAt = createdAt;
        }
    }
}
