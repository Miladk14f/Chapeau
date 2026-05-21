using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByOrderId(int orderId);
        void AddComment(Comment comment);
        void DeleteComment(int id);
    }
}
