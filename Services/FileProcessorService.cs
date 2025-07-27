using ChunxApp.Core.Interfaces;
using ChunxApp.Core.Models;
using ChunxApp.Helpers;
using Microsoft.Extensions.Logging;

namespace ChunxApp.Services;

/// <summary>
/// Service responsible for processing files: chunking, storing chunks and saving metadata.
/// </summary>
public class FileProcessorService
{
    private readonly ILogger<FileProcessorService> _logger;
    private readonly IMetadataRepository _metadataRepository;
    private readonly IEnumerable<IStorageProvider> _storageProviders;

    public FileProcessorService(
        ILogger<FileProcessorService> logger,
        IMetadataRepository metadataRepository,
        IEnumerable<IStorageProvider> storageProviders)
    {
        _logger = logger;
        _metadataRepository = metadataRepository;
        _storageProviders = storageProviders;
    }

    public async Task ProcessFileAsync(string filePath, int chunkSize = 1024 * 1024)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogError("File not found: {FilePath}", filePath);
            return;
        }

        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var checksum = ChecksumHelper.CalculateChecksum(fileBytes);
        var fileInfo = new FileInfo(filePath);

        var fileMetadata = new FileMetadata
        {
            Id = Guid.NewGuid(),
            FileName = fileInfo.Name,
            Size = fileBytes.Length,
            Checksum = checksum,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Processing file: {FileName}", fileMetadata.FileName);

        await _metadataRepository.SaveFileMetadataAsync(fileMetadata);

        int totalChunks = (int)Math.Ceiling((double)fileBytes.Length / chunkSize);

        for (int i = 0; i < totalChunks; i++)
        {
            int offset = i * chunkSize;
            int length = Math.Min(chunkSize, fileBytes.Length - offset);
            byte[] chunkData = new byte[length];
            Array.Copy(fileBytes, offset, chunkData, 0, length);

            var chunk = new ChunkMetadata
            {
                Id = Guid.NewGuid(),
                FileId = fileMetadata.Id,
                Index = i,
                Size = length
            };

            // Use first available storage provider
            var provider = _storageProviders.First();
            var path = await provider.StoreAsync(chunk, chunkData);
            chunk.Path = path;

            await _metadataRepository.SaveChunkMetadataAsync(chunk);
            _logger.LogInformation("Saved chunk {Index}/{Total}", i + 1, totalChunks);
        }

        _logger.LogInformation("Finished processing file: {FileName}", fileMetadata.FileName);
    }
}
