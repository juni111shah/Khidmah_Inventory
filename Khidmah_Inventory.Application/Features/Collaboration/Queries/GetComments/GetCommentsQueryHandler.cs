using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;

namespace Khidmah_Inventory.Application.Features.Collaboration.Queries.GetComments;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCommentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<CommentDto>>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<CommentDto>>.Failure("Company context is required");

        var comments = await _context.Comments
            .Include(c => c.Replies)
            .Where(c => c.CompanyId == companyId.Value &&
                !c.IsDeleted &&
                c.EntityType == request.EntityType &&
                c.EntityId == request.EntityId &&
                c.ParentCommentId == null) // Only top-level comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                EntityType = c.EntityType,
                EntityId = c.EntityId,
                Content = c.Content,
                ParentCommentId = c.ParentCommentId,
                IsEdited = c.IsEdited,
                EditedAt = c.EditedAt,
                UserId = c.UserId,
                UserName = c.UserName,
                CreatedAt = c.CreatedAt,
                TimeAgo = GetTimeAgo(c.CreatedAt),
                Replies = c.Replies
                    .Where(r => !r.IsDeleted)
                    .OrderBy(r => r.CreatedAt)
                    .Select(r => new CommentDto
                    {
                        Id = r.Id,
                        EntityType = r.EntityType,
                        EntityId = r.EntityId,
                        Content = r.Content,
                        ParentCommentId = r.ParentCommentId,
                        IsEdited = r.IsEdited,
                        EditedAt = r.EditedAt,
                        UserId = r.UserId,
                        UserName = r.UserName,
                        CreatedAt = r.CreatedAt,
                        TimeAgo = GetTimeAgo(r.CreatedAt)
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<List<CommentDto>>.Success(comments);
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year(s) ago";
        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month(s) ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day(s) ago";
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour(s) ago";
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
        return "Just now";
    }
}

