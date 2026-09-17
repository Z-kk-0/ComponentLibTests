namespace ComponentLibTests.Domain.Entities;

/// <summary>Master entity — the internal entity (customer/person) being screened. Root of the 2-level master-detail chain.</summary>
public class EntityInternEntity
{
    public int Id { get; set; }
    public string Entity { get; set; } = null!;
    public string? Label { get; set; }
    public int? EntityInternRiskLevelId { get; set; }
    public bool Active { get; set; } = true;
    public DateTime Created { get; set; }
    public DateTime? UpdatedDateMatcher { get; set; }

    public EntityInternRiskLevel? EntityInternRiskLevel { get; set; }

    public ICollection<EntityMatchResult> EntityMatchResults { get; set; } = new List<EntityMatchResult>();
}
