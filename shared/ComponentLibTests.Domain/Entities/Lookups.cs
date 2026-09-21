using Microsoft.AspNetCore.Identity;

namespace ComponentLibTests.Domain.Entities;

public class EntityInternRiskLevel
{
    public int Id { get; set; }
    public string RiskLevel { get; set; } = null!;
    public short Sort { get; set; }
    public bool Active { get; set; } = true;

    public ICollection<EntityInternEntity> EntityInternEntities { get; set; } = new List<EntityInternEntity>();
}

public class EntityMatchStatus
{
    public int Id { get; set; }
    public string Status { get; set; } = null!;
    public short Sort { get; set; }
    public bool Active { get; set; } = true;

    public ICollection<EntityMatchResult> EntityMatchResults { get; set; } = new List<EntityMatchResult>();
    public ICollection<MatchResultStateChange> MatchResultStateChanges { get; set; } = new List<MatchResultStateChange>();
}

public class ProfileRiskLevel
{
    public int Id { get; set; }
    public string RiskLevel { get; set; } = null!;
    public short Sort { get; set; }
    public bool Active { get; set; } = true;

    public ICollection<ProfileExtern> ProfilesExtern { get; set; } = new List<ProfileExtern>();
}

public class EntitySource
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Active { get; set; } = true;

    public ICollection<ProfileExtern> ProfilesExtern { get; set; } = new List<ProfileExtern>();
}

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = null!;

    public ICollection<MatchResultStateChange> MatchResultStateChanges { get; set; } = new List<MatchResultStateChange>();
}
