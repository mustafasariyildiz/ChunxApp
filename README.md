# ChunxApp

**ChunxApp** is a .NET 8 Console Application designed to split large files into smaller chunks, distribute them across pluggable storage providers, and reassemble them while ensuring data integrity through checksum validation.

## ✅ Features

- 🔹 Dynamically splits single or multiple files into chunks based on file size.
- 🔹 Uses a dynamic and optimal chunk size (between **1MB** and **8MB**) determined by the file's total size.
- 🔹 Pluggable storage provider architecture via `IStorageProvider` interface.
- 🔹 Persists metadata for each file and chunk using SQLite.
- 🔹 Reassembles original files from chunks and verifies using SHA256 checksum.
- 🔹 Built with clean architecture, SOLID principles, and full dependency injection.
- 🔹 Fully logged file processing and restore operations.

## 📂 Project Structure

```
ChunxApp/
│
├── Core/                    # Interfaces & Models
│   ├── Interfaces/          # IStorageProvider, IMetadataRepository
│   └── Models/              # FileMetadata, ChunkMetadata
│
├── Infrastructure/
│   ├── Data/                # SQLiteMetadataRepository
│   └── Storage/             # FileSystemStorageProvider
│
├── Services/                # FileProcessorService, RestoreService
├── Helpers/                 # ChecksumHelper, ChunkHelper, PathHelper
├── Program.cs               # Main console interface
└── README.md
```

## 🔧 Getting Started

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Setup

```bash
git clone https://github.com/mustafasariyildiz/ChunxApp.git
cd ChunxApp
dotnet restore
dotnet build
```

## 🚀 Running the App

```bash
dotnet run
```

### Console Menu

```
Select an option:
1 - Upload File
2 - Upload Multiple Files
3 - Restore File
0 - Exit
```

- **Upload File**: Prompts for a file path, splits the file into chunks, stores each chunk, and logs progress.
- **Upload Multiple Files**: Prompts for multiple file paths (comma-separated), processes each file individually, and logs progress per file.
- **Restore File**: Lists available files, prompts for ID, reassembles the file from chunks, and verifies checksum.

## 🧠 Chunking Logic

- The application dynamically calculates an optimal chunk size based on file size:
  - Files smaller than 10MB → **1MB chunks**
  - Files up to 100MB → **4MB chunks**
  - Larger files → **8MB chunks**

This logic ensures both performance and scalability.

## 📁 File Storage

- Chunked files are stored under: `files/chunks/`
- Reassembled files are saved under: `files/restored/`
- Metadata is stored in a local SQLite DB at:
  - macOS: `~/Library/Application Support/ChunxApp/metadata.db`
  - Windows/Linux: `%AppData%/ChunxApp/metadata.db`

## 🧩 Extensibility

You can easily add custom storage providers by implementing:

```csharp
public interface IStorageProvider
{
    Task<string> StoreAsync(ChunkMetadata chunk, byte[] data);
    Task<byte[]> RetrieveAsync(string path);
}
```

Then register them using DI in `Program.cs`.

## 🧪 Tests & Validation

- Checksum validation is performed after reassembling each file to ensure integrity.
- Each chunk operation and file processing step is logged.

## 📌 Notes

- The current implementation uses only the first available `IStorageProvider`. You can easily extend it to distribute chunks across multiple providers.
- This project focuses on demonstrating clean, extensible backend architecture rather than UI or distributed networking.

## 📃 License

MIT © Mustafa Sarıyıldız
