using ChunxApp.Core.Interfaces;
using ChunxApp.Core.Models;
using ChunxApp.Helpers;
using Microsoft.Extensions.Logging;

namespace ChunxApp.Infrastructure.Storage;

/// <summary>
/// Implements IStorageProvider by saving chunks as files in a local folder.
/// </summary>
public class FileSystemStorageProvider : IStorageProvider
{
    private readonly ILogger<FileSystemStorageProvider> _logger;

    public FileSystemStorageProvider(ILogger<FileSystemStorageProvider> logger)
    {
        _logger = logger;
    }

    public async Task<string> StoreAsync(ChunkMetadata chunk, byte[] data)
    {
        var chunksDir = PathHelper.GetChunksDirectory();
        Directory.CreateDirectory(chunksDir);

        var filePath = Path.Combine(chunksDir, $"{chunk.Id}.chunk");

        await File.WriteAllBytesAsync(filePath, data);
        _logger.LogInformation("Saved chunk file: {FilePath}", filePath);

        return filePath;
    }

    public async Task<byte[]> RetrieveAsync(ChunkMetadata chunk)
    {
        var filePath = Path.Combine(PathHelper.GetChunksDirectory(), $"{chunk.Id}.chunk");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Chunk file not found: {FilePath}", filePath);
            throw new FileNotFoundException($"Chunk file not found: {filePath}");
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    public bool CanRetrieve(ChunkMetadata chunk)
    {
        var filePath = Path.Combine(PathHelper.GetChunksDirectory(), $"{chunk.Id}.chunk");
        return File.Exists(filePath);
    }
}
