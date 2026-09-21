using ComponentLibTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComponentLibTests.Data;

/// <summary>Fills the screening tables with a production-scale amount of random data for testing/paging.</summary>
public static class DbSeeder
{
    private static readonly string[] FirstNames =
    [
        "John", "Jane", "Michael", "Sarah", "David", "Laura", "Robert", "Maria", "James", "Anna",
        "Thomas", "Elena", "Ahmed", "Fatima", "Wei", "Li", "Ivan", "Olga", "Carlos", "Sofia",
        "Max", "Julia", "Peter", "Nina", "Hassan", "Layla", "Chen", "Mei", "Boris", "Katarina"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Johnson", "Müller", "Garcia", "Kim", "Wang", "Petrov", "Novak", "Andersson", "Rossi",
        "Dubois", "Silva", "Khan", "Yilmaz", "Nowak", "Ivanov", "Schmidt", "Fischer", "Weber", "Meyer",
        "Costa", "Santos", "Popov", "Kovac", "Larsen", "Berg", "Haddad", "Abdullah", "Chan", "Park"
    ];

    private static readonly string[] Nationalities =
    [
        "German", "American", "British", "French", "Chinese", "Russian", "Brazilian", "Turkish",
        "Indian", "Nigerian", "Egyptian", "Polish", "Spanish", "Italian", "Swedish", "Korean"
    ];

    public static async Task SeedAsync(AppDbContext db, int internCount = 5_000, int externCount = 5_000, int matchCount = 100_000)
    {
        if (await db.EntityMatchResults.AnyAsync())
        {
            return;
        }

        var random = new Random(42);

        var internRiskLevels = new[] { "Low", "Medium", "High", "Critical" }
            .Select((name, i) => new EntityInternRiskLevel { RiskLevel = name, Sort = (short)i })
            .ToList();
        var matchStatuses = new[] { "New", "In Review", "Confirmed", "False Positive" }
            .Select((name, i) => new EntityMatchStatus { Status = name, Sort = (short)i })
            .ToList();
        var profileRiskLevels = new[] { "Low", "Medium", "High" }
            .Select((name, i) => new ProfileRiskLevel { RiskLevel = name, Sort = (short)i })
            .ToList();
        var sources = new[] { "OFAC SDN", "EU Sanctions", "UN Consolidated List", "UK OFSI", "Interpol Red Notice" }
            .Select(name => new EntitySource { Name = name })
            .ToList();

        db.EntityInternRiskLevels.AddRange(internRiskLevels);
        db.EntityMatchStatuses.AddRange(matchStatuses);
        db.ProfileRiskLevels.AddRange(profileRiskLevels);
        db.EntitySources.AddRange(sources);
        await db.SaveChangesAsync();

        db.ChangeTracker.AutoDetectChangesEnabled = false;

        var internIds = await SeedInternEntitiesAsync(db, internCount, internRiskLevels.Select(r => r.Id).ToArray(), random);
        var externIds = await SeedExternProfilesAsync(db, externCount, profileRiskLevels.Select(r => r.Id).ToArray(), sources.Select(s => s.Id).ToArray(), random);
        await SeedMatchResultsAsync(db, matchCount, internIds, externIds, matchStatuses.Select(s => s.Id).ToArray(), random);

        db.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    private static async Task<List<int>> SeedInternEntitiesAsync(AppDbContext db, int count, int[] riskLevelIds, Random random)
    {
        var ids = new List<int>(count);
        const int batchSize = 2_000;

        for (var start = 0; start < count; start += batchSize)
        {
            var batch = new List<EntityInternEntity>(batchSize);
            var end = Math.Min(start + batchSize, count);

            for (var i = start; i < end; i++)
            {
                var fullName = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}";
                batch.Add(new EntityInternEntity
                {
                    Entity = fullName,
                    Label = $"Customer {i + 1}",
                    EntityInternRiskLevelId = riskLevelIds[random.Next(riskLevelIds.Length)],
                    Active = random.Next(100) < 95,
                    Created = RandomDate(random)
                });
            }

            db.EntityInternEntities.AddRange(batch);
            await db.SaveChangesAsync();
            ids.AddRange(batch.Select(e => e.Id));
            db.ChangeTracker.Clear();
        }

        return ids;
    }

    private static async Task<List<int>> SeedExternProfilesAsync(AppDbContext db, int count, int[] riskLevelIds, int[] sourceIds, Random random)
    {
        var ids = new List<int>(count);
        const int batchSize = 2_000;

        for (var start = 0; start < count; start += batchSize)
        {
            var batch = new List<ProfileExtern>(batchSize);
            var end = Math.Min(start + batchSize, count);

            for (var i = start; i < end; i++)
            {
                var firstName = FirstNames[random.Next(FirstNames.Length)];
                var lastName = LastNames[random.Next(LastNames.Length)];
                batch.Add(new ProfileExtern
                {
                    Entity = $"{firstName} {lastName}",
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = RandomDate(random, yearsBack: 80).Date,
                    Nationality = Nationalities[random.Next(Nationalities.Length)],
                    Gender = random.Next(2) == 0 ? "Male" : "Female",
                    ProfileRiskLevelId = riskLevelIds[random.Next(riskLevelIds.Length)],
                    EntitySourceId = sourceIds[random.Next(sourceIds.Length)],
                    Created = RandomDate(random)
                });
            }

            db.ProfilesExtern.AddRange(batch);
            await db.SaveChangesAsync();
            ids.AddRange(batch.Select(p => p.Id));
            db.ChangeTracker.Clear();
        }

        return ids;
    }

    private static async Task SeedMatchResultsAsync(AppDbContext db, int count, List<int> internIds, List<int> externIds, int[] statusIds, Random random)
    {
        const int batchSize = 5_000;

        for (var start = 0; start < count; start += batchSize)
        {
            var batch = new List<EntityMatchResult>(batchSize);
            var end = Math.Min(start + batchSize, count);

            for (var i = start; i < end; i++)
            {
                batch.Add(new EntityMatchResult
                {
                    EntityInternEntitiesId = internIds[random.Next(internIds.Count)],
                    ProfilesExternId = externIds[random.Next(externIds.Count)],
                    MatchingValue = Math.Round(40 + random.NextDouble() * 60, 2),
                    EntityMatchStatusId = statusIds[random.Next(statusIds.Length)],
                    Created = RandomDate(random)
                });
            }

            db.EntityMatchResults.AddRange(batch);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }
    }

    private static DateTime RandomDate(Random random, int yearsBack = 2)
    {
        return DateTime.UtcNow.AddDays(-random.Next(0, 365 * yearsBack)).AddSeconds(-random.Next(0, 86_400));
    }
}
