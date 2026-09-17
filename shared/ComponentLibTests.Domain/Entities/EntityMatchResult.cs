namespace ComponentLibTests.Domain.Entities;

/// <summary>Detail level 1 — a match between an internal entity and an external watchlist profile.</summary>
public class EntityMatchResult
{
    public int Id { get; set; }
    public int EntityInternEntitiesId { get; set; }
    public int ProfilesExternId { get; set; }
    public double MatchingValue { get; set; }
    public int EntityMatchStatusId { get; set; }
    public string? Comment { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public EntityInternEntity? EntityInternEntity { get; set; }
    public ProfileExtern? ProfileExtern { get; set; }
    public EntityMatchStatus? EntityMatchStatus { get; set; }

    public ICollection<MatchResultStateChange> MatchResultStateChanges { get; set; } = new List<MatchResultStateChange>();
}
