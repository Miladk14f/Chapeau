using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ICommentService
    {
        List<Comment> GetByOrderId(int orderId);
        void Add(Comment comment);
        void Delete(int id);
    }
}
