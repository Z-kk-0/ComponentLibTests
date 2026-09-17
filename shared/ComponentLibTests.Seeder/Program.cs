using System.Data;
using Bogus;
using ComponentLibTests.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var options = ParseArgs(args);

Console.WriteLine($"Connection: {options.ConnectionString}");
Console.WriteLine($"Targets: ProfilesExtern={options.Profiles:N0}, EntityInternEntities={options.Subjects:N0}, EntityMatchResults={options.Matches:N0}, MatchResultStateChanges={options.Events:N0}");

var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(options.ConnectionString).Options;
await using (var ctx = new AppDbContext(dbOptions))
{
    Console.WriteLine("Applying migrations...");
    await ctx.Database.MigrateAsync();

    if (options.Reset)
    {
        Console.WriteLine("Resetting existing data...");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM MatchResultStateChanges");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM EntityMatchResults");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM EntityInternEntities");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM ProfilesExtern");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM ApplicationUsers");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM EntityMatchStatuses");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM EntityInternRiskLevels");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM ProfileRiskLevels");
        await ctx.Database.ExecuteSqlRawAsync("DELETE FROM EntitySources");
    }

    var existingSubjects = await ctx.EntityInternEntities.CountAsync();
    if (existingSubjects > 0 && !options.Reset && !options.OnlyEvents)
    {
        Console.WriteLine($"Database already contains {existingSubjects:N0} EntityInternEntities. Use --reset to reseed from scratch. Exiting.");
        return;
    }
}

Randomizer.Seed = new Random(42);
var faker = new Faker();

await using var conn = new SqlConnection(options.ConnectionString);
await conn.OpenAsync();

List<int> riskLevelIds;
List<int> matchStatusIds;
List<int> profileRiskLevelIds;
List<int> sourceIds;
List<Guid> users;

if (options.OnlyEvents)
{
    Console.WriteLine("--only-events: reusing existing lookups/users, seeding MatchResultStateChanges only.");
    matchStatusIds = await ReadIdsAsync(conn, "EntityMatchStatuses");
    users = await ReadGuidIdsAsync(conn, "ApplicationUsers");
    riskLevelIds = new List<int>();
    profileRiskLevelIds = new List<int>();
    sourceIds = new List<int>();
}
else
{
    // 1. Lookups (small — plain inserts)
    var riskLevels = new[] { "Low", "Medium", "High", "Critical", "Unrated" };
    var matchStatuses = new[] { "New", "InReview", "Escalated", "ConfirmedMatch", "FalsePositive", "Cleared", "Closed", "Reopened" };
    var profileRiskLevels = new[] { "Low", "Medium", "High", "Critical", "Sanctioned" };
    var sources = new[] { "OFAC", "EU Consolidated List", "UN Sanctions", "UK HMT", "Interpol Red Notice",
        "World-Check", "Dow Jones", "PEP Register CH", "PEP Register EU", "Local Watchlist",
        "FATF Grey List", "Adverse Media", "Swiss SECO", "OpenSanctions", "National PEP DB",
        "Reuters", "MediaScan", "Internal Blacklist", "Customs Alert List", "Export Control List" };

    riskLevelIds = await InsertLookupAsync(conn, "EntityInternRiskLevels", riskLevels, "RiskLevel");
    matchStatusIds = await InsertLookupAsync(conn, "EntityMatchStatuses", matchStatuses, "Status");
    profileRiskLevelIds = await InsertLookupAsync(conn, "ProfileRiskLevels", profileRiskLevels, "RiskLevel");
    sourceIds = await InsertSourcesAsync(conn, sources);

    Console.WriteLine("Seeding ApplicationUsers...");
    var userFaker = new Faker<SeedUser>()
        .CustomInstantiator(f =>
        {
            var name = f.Name.FullName();
            return new SeedUser(Guid.NewGuid(), f.Internet.UserName(name).ToLowerInvariant(), name);
        });
    var generatedUsers = userFaker.Generate(50);
    users = generatedUsers.Select(u => u.Id).ToList();
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("UserName", typeof(string));
        table.Columns.Add("DisplayName", typeof(string));
        foreach (var u in generatedUsers) table.Rows.Add(u.Id, u.UserName, u.DisplayName);
        await BulkInsertAsync(conn, "ApplicationUsers", table, useIdentityInsert: false);
    }
}

if (options.OnlyEvents)
{
    await SeedEventsAsync(conn, options, matchStatusIds, users);
    await PrintCountsAsync(conn);
    return;
}

// 2. ProfilesExtern (external watchlist pool)
Console.WriteLine($"Seeding {options.Profiles:N0} ProfilesExtern...");
var profileFaker = new Faker();
await BulkInsertBatchedAsync(conn, "ProfilesExtern", options.Profiles, options.BatchSize, (id, f) =>
{
    var first = f.Name.FirstName();
    var last = f.Name.LastName();
    return new object?[]
    {
        id,
        $"{first} {last}",
        first,
        last,
        f.Date.BetweenDateOnly(new DateOnly(1935, 1, 1), new DateOnly(2005, 12, 31)).ToDateTime(TimeOnly.MinValue),
        f.Address.Country(),
        f.PickRandom(new[] { "Male", "Female" }),
        f.PickRandom(profileRiskLevelIds),
        f.PickRandom(sourceIds),
        f.Lorem.Sentence(10),
        f.Date.Past(8)
    };
}, new[] { "Id", "Entity", "FirstName", "LastName", "DateOfBirth", "Nationality", "Gender", "ProfileRiskLevelId", "EntitySourceId", "Remarks", "Created" },
   new[] { typeof(int), typeof(string), typeof(string), typeof(string), typeof(DateTime), typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(DateTime) });

// 3. EntityInternEntity (master)
Console.WriteLine($"Seeding {options.Subjects:N0} EntityInternEntities...");
await BulkInsertBatchedAsync(conn, "EntityInternEntities", options.Subjects, options.BatchSize, (id, f) =>
{
    var isOrg = f.Random.Bool(0.25f);
    var name = isOrg ? f.Company.CompanyName() : f.Name.FullName();
    return new object?[]
    {
        id,
        name,
        isOrg ? "Organization" : "Person",
        f.PickRandom(riskLevelIds),
        f.Random.Bool(0.92f),
        f.Date.Past(6),
        f.Random.Bool(0.6f) ? f.Date.Recent(180) : (DateTime?)null
    };
}, new[] { "Id", "Entity", "Label", "EntityInternRiskLevelId", "Active", "Created", "UpdatedDateMatcher" },
   new[] { typeof(int), typeof(string), typeof(string), typeof(int), typeof(bool), typeof(DateTime), typeof(DateTime) });

// 4. EntityMatchResult (detail level 1)
Console.WriteLine($"Seeding {options.Matches:N0} EntityMatchResults...");
await BulkInsertBatchedAsync(conn, "EntityMatchResults", options.Matches, options.BatchSize, (id, f) =>
{
    return new object?[]
    {
        id,
        f.Random.Int(1, options.Subjects),
        f.Random.Int(1, options.Profiles),
        Math.Round(f.Random.Double(50, 100), 2),
        f.PickRandom(matchStatusIds),
        f.Random.Bool(0.4f) ? f.Lorem.Sentence(8) : null,
        f.Date.Past(5),
        f.Random.Bool(0.5f) ? f.Date.Recent(90) : (DateTime?)null
    };
}, new[] { "Id", "EntityInternEntitiesId", "ProfilesExternId", "MatchingValue", "EntityMatchStatusId", "Comment", "Created", "Updated" },
   new[] { typeof(int), typeof(int), typeof(int), typeof(double), typeof(int), typeof(string), typeof(DateTime), typeof(DateTime) });

// 5. MatchResultStateChange (detail level 2)
await SeedEventsAsync(conn, options, matchStatusIds, users);

await PrintCountsAsync(conn);

Console.WriteLine("Done.");

static async Task SeedEventsAsync(SqlConnection conn, SeedOptions options, List<int> matchStatusIds, List<Guid> users)
{
    Console.WriteLine($"Seeding {options.Events:N0} MatchResultStateChanges...");
    await BulkInsertBatchedAsync(conn, "MatchResultStateChanges", options.Events, options.BatchSize, (id, f) =>
    {
        return new object?[]
        {
            id,
            f.Random.Int(1, options.Matches),
            f.Date.Past(5),
            f.PickRandom(users),
            f.Random.Bool(0.7f) ? f.Lorem.Sentence(6) : null,
            f.PickRandom(matchStatusIds)
        };
    }, new[] { "Id", "MatchResultId", "UpdateDate", "UpdateUserId", "Comment", "NewMatchStatusId" },
       new[] { typeof(int), typeof(int), typeof(DateTime), typeof(Guid), typeof(string), typeof(int) });
}

static async Task PrintCountsAsync(SqlConnection conn)
{
    Console.WriteLine("Verifying row counts...");
    foreach (var t in new[] { "EntitySources", "EntityInternRiskLevels", "EntityMatchStatuses", "ProfileRiskLevels", "ApplicationUsers", "ProfilesExtern", "EntityInternEntities", "EntityMatchResults", "MatchResultStateChanges" })
    {
        await using var cmd = new SqlCommand($"SELECT COUNT(*) FROM {t}", conn);
        var count = (int)(await cmd.ExecuteScalarAsync())!;
        Console.WriteLine($"  {t}: {count:N0}");
    }
}

static async Task<List<int>> ReadIdsAsync(SqlConnection conn, string table)
{
    var ids = new List<int>();
    await using var cmd = new SqlCommand($"SELECT Id FROM {table}", conn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync()) ids.Add(reader.GetInt32(0));
    return ids;
}

static async Task<List<Guid>> ReadGuidIdsAsync(SqlConnection conn, string table)
{
    var ids = new List<Guid>();
    await using var cmd = new SqlCommand($"SELECT Id FROM {table}", conn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync()) ids.Add(reader.GetGuid(0));
    return ids;
}

static async Task<List<int>> InsertLookupAsync(SqlConnection conn, string table, string[] values, string column)
{
    var ids = new List<int>();
    for (var i = 0; i < values.Length; i++)
    {
        var sort = (short)i;
        await using var cmd = new SqlCommand(
            $"INSERT INTO {table} ({column}, Sort, Active) OUTPUT INSERTED.Id VALUES (@v, @sort, 1)", conn);
        cmd.Parameters.AddWithValue("@v", values[i]);
        cmd.Parameters.AddWithValue("@sort", sort);
        var id = (int)(await cmd.ExecuteScalarAsync())!;
        ids.Add(id);
    }
    return ids;
}

static async Task<List<int>> InsertSourcesAsync(SqlConnection conn, string[] values)
{
    var ids = new List<int>();
    foreach (var v in values)
    {
        await using var cmd = new SqlCommand(
            "INSERT INTO EntitySources (Name, Active) OUTPUT INSERTED.Id VALUES (@v, 1)", conn);
        cmd.Parameters.AddWithValue("@v", v);
        var id = (int)(await cmd.ExecuteScalarAsync())!;
        ids.Add(id);
    }
    return ids;
}

static async Task BulkInsertAsync(SqlConnection conn, string table, DataTable data, bool useIdentityInsert)
{
    if (useIdentityInsert)
        await using (var on = new SqlCommand($"SET IDENTITY_INSERT {table} ON", conn)) { await on.ExecuteNonQueryAsync(); }

    using var bulk = new SqlBulkCopy(conn) { DestinationTableName = table, BatchSize = 20000, BulkCopyTimeout = 0 };
    foreach (DataColumn col in data.Columns) bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
    await bulk.WriteToServerAsync(data);

    if (useIdentityInsert)
        await using (var off = new SqlCommand($"SET IDENTITY_INSERT {table} OFF", conn)) { await off.ExecuteNonQueryAsync(); }
}

static async Task BulkInsertBatchedAsync(SqlConnection conn, string table, int totalRows, int batchSize,
    Func<int, Faker, object?[]> rowFactory, string[] columns, Type[]? columnTypes = null)
{
    await using (var on = new SqlCommand($"SET IDENTITY_INSERT {table} ON", conn)) { await on.ExecuteNonQueryAsync(); }

    var f = new Faker();
    var written = 0;
    while (written < totalRows)
    {
        var batchCount = Math.Min(batchSize, totalRows - written);
        var dt = new DataTable();
        for (var c = 0; c < columns.Length; c++)
            dt.Columns.Add(columns[c], columnTypes?[c] ?? typeof(string));

        for (var i = 0; i < batchCount; i++)
        {
            var id = written + i + 1;
            var row = rowFactory(id, f);
            dt.Rows.Add(row);
        }

        using var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, null) { DestinationTableName = table, BatchSize = 20000, BulkCopyTimeout = 0 };
        foreach (var c in columns) bulk.ColumnMappings.Add(c, c);
        await bulk.WriteToServerAsync(dt);

        written += batchCount;
        Console.WriteLine($"  {table}: {written:N0} / {totalRows:N0}");
    }

    await using (var off = new SqlCommand($"SET IDENTITY_INSERT {table} OFF", conn)) { await off.ExecuteNonQueryAsync(); }
}

static SeedOptions ParseArgs(string[] args)
{
    var opts = new SeedOptions();
    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--connection": opts.ConnectionString = args[++i]; break;
            case "--subjects": opts.Subjects = int.Parse(args[++i]); break;
            case "--matches": opts.Matches = int.Parse(args[++i]); break;
            case "--events": opts.Events = int.Parse(args[++i]); break;
            case "--profiles": opts.Profiles = int.Parse(args[++i]); break;
            case "--batch-size": opts.BatchSize = int.Parse(args[++i]); break;
            case "--reset": opts.Reset = true; break;
            case "--only-events": opts.OnlyEvents = true; break;
        }
    }
    return opts;
}

record SeedUser(Guid Id, string UserName, string DisplayName);

class SeedOptions
{
    public string ConnectionString { get; set; } =
        Environment.GetEnvironmentVariable("COMPONENTLIBTESTS_CONNECTION")
        ?? "Server=localhost,1433;Database=ComponentLibBenchmark;User Id=sa;Password=StrongP@ss!;TrustServerCertificate=True";
    public int Profiles { get; set; } = 50_000;
    public int Subjects { get; set; } = 100_000;
    public int Matches { get; set; } = 500_000;
    public int Events { get; set; } = 1_500_000;
    public int BatchSize { get; set; } = 25_000;
    public bool Reset { get; set; }
    public bool OnlyEvents { get; set; }
}
