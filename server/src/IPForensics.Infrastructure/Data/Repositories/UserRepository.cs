using System.Data.Common;
using IPForensics.Core.Entities;
using IPForensics.Core.Interfaces.Repositories;
using MySqlConnector;


namespace IPForensics.Infrastructure.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private const int DuplicateEntryErrorNumber = 1062;

    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, first_name, last_name, email, password_hash, created_at
            FROM users
            WHERE email = @email
            LIMIT 1;";

        await using var connection = await _connectionFactory.OpenAsync(cancellationToken);
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<bool> TryAddAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO users (id, first_name, last_name, email, password_hash, created_at)
            VALUES (@id, @firstName, @lastName, @email, @passwordHash, @createdAt);";

        await using var connection = await _connectionFactory.OpenAsync(cancellationToken);
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@firstName", user.FirstName);
        command.Parameters.AddWithValue("@lastName", user.LastName);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@createdAt", user.CreatedAtUtc);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
            return true;
        }
        catch (MySqlException ex) when (ex.Number == DuplicateEntryErrorNumber)
        {
            return false;
        }
    }
    
    private static User Map(DbDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            PasswordHash = reader.GetString(4),
            CreatedAtUtc = DateTime.SpecifyKind(reader.GetDateTime(5), DateTimeKind.Utc)
        };
    }
}
