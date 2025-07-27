using ChunxApp.Core.Models;

namespace ChunxApp.Core.Interfaces;

/// <summary>
/// Provides methods for saving and retrieving metadata from persistent storage.
/// </summary>
public interface IMetadataRepository
{
    Task SaveFileMetadataAsync(FileMetadata file);
    Task SaveChunkMetadataAsync(ChunkMetadata chunk);
    Task<FileMetadata?> GetFileMetadataAsync(Guid fileId);
    Task<IEnumerable<ChunkMetadata>> GetChunkMetadataAsync(Guid fileId);
    Task<IEnumerable<FileMetadata>> GetAllFileMetadataAsync();
}
