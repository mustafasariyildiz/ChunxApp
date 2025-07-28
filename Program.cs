using ChunxApp.Core.Interfaces;
using ChunxApp.Helpers;
using ChunxApp.Infrastructure.Data;
using ChunxApp.Infrastructure.Storage;
using ChunxApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Logging configuration
services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

// Dependency injection registrations
services.AddSingleton<IStorageProvider, FileSystemStorageProvider>();
services.AddSingleton<IMetadataRepository, SQLiteMetadataRepository>();
services.AddSingleton<FileProcessorService>();
services.AddSingleton<RestoreService>();

var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();
var fileProcessor = provider.GetRequiredService<FileProcessorService>();
var restoreService = provider.GetRequiredService<RestoreService>();
var repository = provider.GetRequiredService<IMetadataRepository>();

logger.LogInformation("[Init] DB Path: {DbPath}", PathHelper.GetDatabasePath());

while (true)
{
    Console.WriteLine("Select an option:");
    Console.WriteLine("1 - Upload File");
    Console.WriteLine("2 - Upload Multiple Files");
    Console.WriteLine("3 - Restore File");
    Console.WriteLine("0 - Exit");
    Console.Write("Your choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Enter file path: ");
        var path = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(path))
            await fileProcessor.ProcessFileAsync(path!);
    }
    else if (choice == "2")
    {
        Console.WriteLine("Enter file paths separated by semicolon (;):");
        var input = Console.ReadLine();
        var filePaths = input?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? Array.Empty<string>();

        if (filePaths.Any())
        {
            await fileProcessor.ProcessMultipleFilesAsync(filePaths);
        }
    }
    else if (choice == "3")
    {
        var allFiles = await repository.GetAllFileMetadataAsync();
        if (!allFiles.Any())
        {
            Console.WriteLine("No files found.");
            continue;
        }

        Console.WriteLine("Available files:");
        foreach (var file in allFiles)
        {
            Console.WriteLine($"- {file.FileName} ({file.Size} bytes) => {file.Id}");
        }

        Console.Write("Enter File ID to restore: ");
        var idInput = Console.ReadLine();

        if (Guid.TryParse(idInput, out var fileId))
        {
            await restoreService.RestoreFileAsync(fileId);
        }
        else
        {
            Console.WriteLine("Invalid GUID.");
        }
    }
    else if (choice == "0")
    {
        break;
    }
}
