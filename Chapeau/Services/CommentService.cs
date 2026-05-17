using System.Collections.Generic;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Service
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepo;

        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public List<Comment> GetOrderComments(int orderId)
        {
            return _commentRepo.GetCommentsByOrderIdAsync(orderId);
        }

        public void SaveComment(Comment comment)
        {
            _commentRepo.AddCommentAsync(comment);
        }
    }
}