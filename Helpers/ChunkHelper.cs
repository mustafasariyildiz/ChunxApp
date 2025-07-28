namespace ChunxApp.Helpers;

/// <summary>
/// Provides utility methods for determining optimal chunk sizes based on file size.
/// </summary>
public static class ChunkHelper
{
    public static int GetOptimalChunkSize(long fileSizeBytes)
    {
        if (fileSizeBytes < 10 * 1024 * 1024) // < 10MB
            return 1 * 1024 * 1024; // 1 MB

        if (fileSizeBytes < 100 * 1024 * 1024) // < 100MB
            return 4 * 1024 * 1024; // 4 MB

        return 8 * 1024 * 1024; // 8 MB for larger files
    }
}
