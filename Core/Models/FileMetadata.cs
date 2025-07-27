namespace ChunxApp.Core.Models;

/// <summary>
/// Represents metadata information for an uploaded file.
/// </summary>
public class FileMetadata
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
