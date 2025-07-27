using ChunxApp.Core.Models;

namespace ChunxApp.Core.Interfaces
{
    /// <summary>
    /// Abstracts storage providers for saving and reading file chunks.
    /// </summary>
    public interface IStorageProvider
    {
        Task<string> StoreAsync(ChunkMetadata chunk, byte[] data);
        Task<byte[]> RetrieveAsync(ChunkMetadata chunk);
        bool CanRetrieve(ChunkMetadata chunk);
    }
}
