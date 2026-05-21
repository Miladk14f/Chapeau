using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        public List<Comment> GetAllComments()
        {
            return _repository.GetAllComments();
        }

        public List<Comment> GetCommentsByOrderId(int orderId)
        {
            return _repository.GetCommentsByOrderId(orderId);
        }

        public void AddComment(Comment comment)
        {
            _repository.AddComment(comment);
        }

        public void DeleteComment(int id)
        {
            _repository.DeleteComment(id);
        }
    }
}
