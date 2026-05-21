using System.Collections.Generic;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByOrderIdAsync(int orderId);
        void AddCommentAsync(Comment comment);
    }
}