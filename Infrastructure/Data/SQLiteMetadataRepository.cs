using ChunxApp.Core.Interfaces;
using ChunxApp.Core.Models;
using ChunxApp.Helpers;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace ChunxApp.Infrastructure.Data;

/// <summary>
/// SQLite implementation of IMetadataRepository.
/// Handles saving and retrieving file and chunk metadata.
/// </summary>
public class SQLiteMetadataRepository : IMetadataRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SQLiteMetadataRepository> _logger;

    public SQLiteMetadataRepository(ILogger<SQLiteMetadataRepository> logger)
    {
        _logger = logger;
        var dbPath = PathHelper.GetDatabasePath();
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS FileMetadata (
                Id TEXT PRIMARY KEY,
                FileName TEXT NOT NULL,
                Size INTEGER NOT NULL,
                Checksum TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS ChunkMetadata (
                Id TEXT PRIMARY KEY,
                FileId TEXT NOT NULL,
                [Index] INTEGER NOT NULL,
                Path TEXT NOT NULL,
                Size INTEGER NOT NULL
            );
        ");

        _logger.LogInformation("Initialized SQLite DB at {Path}", PathHelper.GetDatabasePath());
    }

    public async Task SaveFileMetadataAsync(FileMetadata file)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO FileMetadata (Id, FileName, Size, Checksum, CreatedAt) 
            VALUES (@Id, @FileName, @Size, @Checksum, @CreatedAt)
        ",
        new {
            Id = file.Id.ToString(),
            file.FileName,
            file.Size,
            file.Checksum,
            file.CreatedAt
        });
    }

    public async Task SaveChunkMetadataAsync(ChunkMetadata chunk)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO ChunkMetadata (Id, FileId, [Index], Path, Size)
            VALUES (@Id, @FileId, @Index, @Path, @Size)
        ",
        new {
            Id = chunk.Id.ToString(),
            FileId = chunk.FileId.ToString(),
            chunk.Index,
            chunk.Path,
            chunk.Size
        });
    }

    public async Task<FileMetadata?> GetFileMetadataAsync(Guid fileId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var row = await connection.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT * FROM FileMetadata WHERE Id = @Id", new { Id = fileId.ToString() });

        if (row == null) return null;

        return new FileMetadata
        {
            Id = Guid.Parse((string)row.Id),
            FileName = row.FileName,
            Size = (long)row.Size,
            Checksum = row.Checksum,
            CreatedAt = DateTime.Parse((string)row.CreatedAt)
        };
    }

    public async Task<IEnumerable<ChunkMetadata>> GetChunkMetadataAsync(Guid fileId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var rows = await connection.QueryAsync<dynamic>(
            "SELECT * FROM ChunkMetadata WHERE FileId = @FileId ORDER BY [Index]", new { FileId = fileId.ToString() });

        return rows.Select(row => new ChunkMetadata
        {
            Id = Guid.Parse((string)row.Id),
            FileId = Guid.Parse((string)row.FileId),
            Index = (int)row.Index,
            Path = row.Path,
            Size = (long)row.Size
        });
    }

    public async Task<IEnumerable<FileMetadata>> GetAllFileMetadataAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        var rows = await connection.QueryAsync<dynamic>("SELECT * FROM FileMetadata");

        return rows.Select(row => new FileMetadata
        {
            Id = Guid.Parse((string)row.Id),
            FileName = row.FileName,
            Size = (long)row.Size,
            Checksum = row.Checksum,
            CreatedAt = DateTime.Parse((string)row.CreatedAt)
        });
    }
}
