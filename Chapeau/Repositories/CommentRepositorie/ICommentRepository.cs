using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetByOrderId(int orderId);
        void Add(Comment comment);
        void Delete(int id);
    }
}
