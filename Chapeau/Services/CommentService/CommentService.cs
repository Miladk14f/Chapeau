using Chapeau.Models;
using Chapeau.Models.Enums;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;
        private readonly IOrderRepository _orderRepository;

        public CommentService(ICommentRepository repository, IOrderRepository orderRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
        }

        public void AddCommentForTable(int tableId, string type, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            Order order = _orderRepository.GetActiveOrderByTableId(tableId);

            CommentType commentType = Enum.TryParse(type, ignoreCase: true, out CommentType parsed)
                ? parsed
                : CommentType.Comment;

            Comment comment = new Comment
            {
                Order = order,
                Type = commentType,
                Text = text.Trim(),
                CreatedAt = DateTime.Now
            };

            _repository.AddComment(comment);
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
