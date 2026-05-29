using Chapeau.Models.Enums;

namespace Chapeau.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public ECommentType Type { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public Order? Order { get; set; }

        public Comment() { }

        public Comment(int commentId, ECommentType type, string text, DateTime createdAt)
        {
            CommentId = commentId;
            Type = type;
            Text = text;
            CreatedAt = createdAt;
        }
    }
}
