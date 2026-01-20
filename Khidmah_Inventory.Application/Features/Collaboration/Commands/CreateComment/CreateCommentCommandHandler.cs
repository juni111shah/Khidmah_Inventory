using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Collaboration.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCommentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CommentDto>.Failure("Company context is required");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        var comment = new Comment(
            companyId.Value,
            request.EntityType,
            request.EntityId,
            request.Content,
            _currentUser.UserId,
            user?.UserName,
            request.ParentCommentId);

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new CommentDto
        {
            Id = comment.Id,
            EntityType = comment.EntityType,
            EntityId = comment.EntityId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            UserId = comment.UserId,
            UserName = comment.UserName,
            CreatedAt = comment.CreatedAt,
            TimeAgo = "Just now"
        };

        return Result<CommentDto>.Success(dto);
    }
}

