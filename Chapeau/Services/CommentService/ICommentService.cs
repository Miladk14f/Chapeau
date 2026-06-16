using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ICommentService
    {
        List<Comment> GetAllComments();
        List<Comment> GetCommentsByOrderId(int orderId);
        void AddComment(Comment comment);
        void AddCommentForTable(int tableId, string type, string text);
        void DeleteComment(int id);
    }
}
