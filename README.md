# ChunxApp

**ChunxApp** is a .NET 8 Console Application designed for splitting large files into chunks, storing them via pluggable storage providers (e.g., filesystem), and reassembling them with checksum validation.

> ✅ Ideal for backup systems, distributed storage experiments, and demonstrating clean, extensible .NET backend architecture.

---

## 🧩 Features

- ✅ Automatic chunking of single large files
- ✅ Pluggable storage architecture via `IStorageProvider`
- ✅ Chunk metadata and file information persisted to SQLite
- ✅ SHA256 checksum verification during restore
- ✅ Clean OOP & SOLID-based architecture
- ✅ Built-in logging via `ILogger`
- ✅ 1MB default chunk size (configurable)
