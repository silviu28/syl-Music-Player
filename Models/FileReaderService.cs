using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NAudio;

namespace MusicPlayer.Models;

public static class FileReaderService
{
    public static string CurrentDirectory { get; set; } = ".";

    public static ObservableCollection<string> GetAll()
    {
        var files = Directory.GetFiles(CurrentDirectory)
                                             .Where(file => file.EndsWith(".mp3")
                                             || file.EndsWith(".wav")
                                             || file.EndsWith(".wma")
                                             || file.EndsWith(".aac")
                                             || file.EndsWith(".aiff")
                                             || file.EndsWith(".mp4"));

        return new ObservableCollection<string>(files);
    }
}