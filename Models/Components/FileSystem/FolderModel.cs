using System;
using System.Collections.Generic;
using System.IO;


namespace warkbench.Models;

public class FolderModel
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.MinValue;
    public List<FolderModel> SubFolders { get; set; } = [];
    public List<FileModel> Files { get; set; } = [];

    public FolderModel(string fullPath)
    {
        Name = new DirectoryInfo(fullPath).Name;
        FullPath = fullPath;
        LastModified = Directory.GetLastWriteTime(fullPath);
    }

    public FolderModel() { }
}