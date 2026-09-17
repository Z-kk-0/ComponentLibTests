namespace ComponentLibTests.Domain.Entities;

/// <summary>Detail level 2 — status-change history of a match result.</summary>
public class MatchResultStateChange
{
    public int Id { get; set; }
    public int MatchResultId { get; set; }
    public DateTime UpdateDate { get; set; }
    public Guid UpdateUserId { get; set; }
    public string? Comment { get; set; }
    public int NewMatchStatusId { get; set; }

    public EntityMatchResult? EntityMatchResult { get; set; }
    public ApplicationUser? UpdateUser { get; set; }
    public EntityMatchStatus? NewMatchStatus { get; set; }
}
