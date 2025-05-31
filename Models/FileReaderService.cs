using System.Collections.ObjectModel;
using System.IO;

namespace MusicPlayer.Models;

public static class FileReaderService
{
    public static string CurrentDirectory { get; set; } = ".";
    public static ObservableCollection<string> GetAll()
    {
        var files = Directory.GetFiles(CurrentDirectory);
        return new ObservableCollection<string>(files);
    }
}