using System.Data.Common;
using IPForensics.Core.Entities;
using IPForensics.Core.Interfaces.Repositories;
using MySqlConnector;

namespace IPForensics.Infrastructure.Data.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ReportRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(StoredReport report, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO ip_forensics_reports (id, user_id, payload, created_at)
            VALUES (@id, @userId, @payload, @createdAt);";

        await using var connection = await _connectionFactory.OpenAsync(cancellationToken);
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", report.Id);
        command.Parameters.AddWithValue("@userId", report.UserId);
        command.Parameters.AddWithValue("@payload", report.Payload);
        command.Parameters.AddWithValue("@createdAt", report.CreatedAtUtc);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<StoredReport?> GetByIdAsync(Guid userId, Guid reportId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, user_id, payload, created_at
            FROM ip_forensics_reports
            WHERE id = @id AND user_id = @userId
            LIMIT 1;";

        await using var connection = await _connectionFactory.OpenAsync(cancellationToken);
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", reportId);
        command.Parameters.AddWithValue("@userId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<(IReadOnlyList<StoredReport> Items, int TotalCount)> ListAsync(
        Guid userId, int skip, int take, CancellationToken cancellationToken)
    {
        const string pageSql = @"
            SELECT id, user_id, payload, created_at
            FROM ip_forensics_reports
            WHERE user_id = @userId
            ORDER BY created_at DESC, id DESC
            LIMIT @take OFFSET @skip;";

        const string countSql = @"
            SELECT COUNT(*) FROM ip_forensics_reports WHERE user_id = @userId;";

        await using var connection = await _connectionFactory.OpenAsync(cancellationToken);

        var items = new List<StoredReport>();
        await using (var pageCommand = new MySqlCommand(pageSql, connection))
        {
            pageCommand.Parameters.AddWithValue("@userId", userId);
            pageCommand.Parameters.AddWithValue("@take", take);
            pageCommand.Parameters.AddWithValue("@skip", skip);

            await using var reader = await pageCommand.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(Map(reader));
            }
        }

        await using var countCommand = new MySqlCommand(countSql, connection);
        countCommand.Parameters.AddWithValue("@userId", userId);
        var totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));

        return (items, totalCount);
    }
    
    private static StoredReport Map(DbDataReader reader)
    {
        return new StoredReport
        {
            Id = reader.GetGuid(0),
            UserId = reader.GetGuid(1),
            Payload = (byte[])reader.GetValue(2),
            CreatedAtUtc = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc)
        };
    }
}
