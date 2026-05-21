using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public ECommentType Type { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order? Order { get; set; }

        public Comment() { }

        public Comment(int id, int orderId, ECommentType type, string text, DateTime createdAt)
        {
            Id = id;
            OrderId = orderId;
            Type = type;
            Text = text;
            CreatedAt = createdAt;
        }
    }
}
