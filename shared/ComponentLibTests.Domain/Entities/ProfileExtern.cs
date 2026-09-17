namespace ComponentLibTests.Domain.Entities;

/// <summary>External watchlist / PEP profile that internal entities are matched against.</summary>
public class ProfileExtern
{
    public int Id { get; set; }
    public string Entity { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public int? ProfileRiskLevelId { get; set; }
    public int EntitySourceId { get; set; }
    public string? Remarks { get; set; }
    public DateTime Created { get; set; }

    public ProfileRiskLevel? ProfileRiskLevel { get; set; }
    public EntitySource? EntitySource { get; set; }

    public ICollection<EntityMatchResult> EntityMatchResults { get; set; } = new List<EntityMatchResult>();
}
