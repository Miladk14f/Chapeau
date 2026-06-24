using Chapeau.Models;

namespace Chapeau.Services
{
    public interface ICommentService
    {
        List<Comment> GetAllComments();
        void AddCommentForTable(int tableId, string type, string text);
    }
}
