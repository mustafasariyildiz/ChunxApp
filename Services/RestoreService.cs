using ChunxApp.Core.Interfaces;
using ChunxApp.Core.Models;
using ChunxApp.Helpers;
using Microsoft.Extensions.Logging;

namespace ChunxApp.Services;

/// <summary>
/// Service responsible for restoring files from chunks.
/// </summary>
public class RestoreService
{
    private readonly ILogger<RestoreService> _logger;
    private readonly IMetadataRepository _metadataRepository;
    private readonly IEnumerable<IStorageProvider> _storageProviders;

    public RestoreService(
        ILogger<RestoreService> logger,
        IMetadataRepository metadataRepository,
        IEnumerable<IStorageProvider> storageProviders)
    {
        _logger = logger;
        _metadataRepository = metadataRepository;
        _storageProviders = storageProviders;
    }

    public async Task RestoreFileAsync(Guid fileId)
    {
        var fileMetadata = await _metadataRepository.GetFileMetadataAsync(fileId);
        if (fileMetadata == null)
        {
            _logger.LogWarning("File metadata not found: {FileId}", fileId);
            return;
        }

        var chunks = (await _metadataRepository.GetChunkMetadataAsync(fileId))
            .OrderBy(c => c.Index)
            .ToList();

        var outputPath = Path.Combine(PathHelper.GetRestoredDirectory(), $"restored_{fileMetadata.FileName}");

        await using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

        foreach (var chunk in chunks)
        {
            var provider = _storageProviders.FirstOrDefault(p => p.CanRetrieve(chunk));
            if (provider == null)
            {
                _logger.LogWarning("No storage provider found for chunk {ChunkId}", chunk.Id);
                continue;
            }

            var chunkData = await provider.RetrieveAsync(chunk);
            await outputStream.WriteAsync(chunkData);
            _logger.LogInformation("Restored chunk {Index}: {Path}", chunk.Index, chunk.Path);
        }

        await outputStream.FlushAsync();
        outputStream.Close();

        var restoredBytes = await File.ReadAllBytesAsync(outputPath);
        var checksum = ChecksumHelper.CalculateChecksum(restoredBytes);

        if (checksum == fileMetadata.Checksum)
        {
            _logger.LogInformation("Restore successful. Checksum verified.");
        }
        else
        {
            _logger.LogError("Restore failed. Checksum mismatch.");
        }
    }
}
