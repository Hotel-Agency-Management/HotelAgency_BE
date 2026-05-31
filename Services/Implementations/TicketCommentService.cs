using Booking.DTO;
using Booking.Enums;
using Booking.Exceptions;
using Booking.Interfaces.Repositories;
using Booking.Interfaces.Services;
using Booking.Models;

namespace Booking.Services
{
    public class TicketCommentService(
        ITicketCommentRepository _commentRepository) : ITicketCommentService
    {
        public async Task<TicketCommentResponse> AddCommentAsync(
            int ticketId, int authorUserId, CreateTicketCommentRequest request)
        {
            ValidateDamageCost(request.CommentType, request.DamageCost);

            var comment = new TicketComment
            {
                TicketId = ticketId,
                AuthorId = authorUserId,
                CommentType = request.CommentType,
                Content = request.Content,
                DamageCost = request.CommentType == CommentType.Damage ? request.DamageCost : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saved = await _commentRepository.AddAsync(comment);
            return new TicketCommentResponse(saved);
        }

        public async Task<PaginatedResponse<TicketCommentResponse>> GetCommentsByTicketAsync(
            int ticketId, TicketCommentListRequest request)
        {
            var comments = (await _commentRepository.GetByTicketIdAsync(
                ticketId, request.PageNumber, request.PageSize)).ToList();
            var totalCount = await _commentRepository.CountByTicketIdAsync(ticketId);

            return new PaginatedResponse<TicketCommentResponse>
            {
                Items = comments.Select(c => new TicketCommentResponse(c)).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<TicketCommentResponse> UpdateCommentAsync(
            int ticketId, int commentId, int requestingUserId, UpdateTicketCommentRequest request)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId)
                ?? throw new TicketCommentNotFoundException(commentId);

            if (comment.AuthorId != requestingUserId)
                throw new TicketCommentForbiddenException();

            if (comment.CommentType == CommentType.Damage && request.DamageCost.HasValue)
            {
                if (request.DamageCost.Value <= 0)
                    throw new TicketCommentValidationException("DamageCost must be greater than 0.");
                comment.DamageCost = request.DamageCost.Value;
            }

            if (request.Content is not null)
                comment.Content = request.Content;

            comment.UpdatedAt = DateTime.UtcNow;

            var updated = await _commentRepository.UpdateAsync(comment);
            return new TicketCommentResponse(updated);
        }

        public async Task DeleteCommentAsync(int ticketId, int commentId, int requestingUserId, bool isAdmin = false)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId)
                ?? throw new TicketCommentNotFoundException(commentId);

            if (!isAdmin && comment.AuthorId != requestingUserId)
                throw new TicketCommentForbiddenException();

            await _commentRepository.DeleteAsync(comment);
        }

        private static void ValidateDamageCost(CommentType type, decimal? cost)
        {
            if (type == CommentType.Damage)
            {
                if (!cost.HasValue)
                    throw new TicketCommentValidationException("DamageCost is required when CommentType is Damage.");
                if (cost.Value <= 0)
                    throw new TicketCommentValidationException("DamageCost must be greater than 0.");
            }
        }
    }
}
