using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ICommentService
    {
        List<Comment> GetCommentsByOrderId(int orderId);
        void AddComment(Comment comment);
        void DeleteComment(int id);
    }
}
