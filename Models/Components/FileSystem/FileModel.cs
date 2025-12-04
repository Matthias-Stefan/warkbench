using System;
using System.IO;
using warkbench.Infrastructure;


namespace warkbench.Models;
public class FileModel
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.MinValue;

    public FileModel(string fullPath)
    {
        Name = UnixPath.GetFileName(fullPath);
        FullPath = fullPath;
        LastModified = File.GetLastWriteTime(fullPath);
    }

    public FileModel() { }
}