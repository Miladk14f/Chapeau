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

        public List<Comment> GetByOrderId(int orderId)
        {
            return _repository.GetByOrderId(orderId);
        }

        public void Add(Comment comment)
        {
            _repository.Add(comment);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
