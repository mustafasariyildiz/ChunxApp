namespace ChunxApp.Core.Models;

/// <summary>
/// Represents metadata information for a single chunk of a file.
/// </summary>
public class ChunkMetadata
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public int Index { get; set; }
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
}
