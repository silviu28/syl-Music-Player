using Avalonia.Media.Imaging;

namespace MusicPlayer.ViewModels;

public partial class PlayerViewModel
{
    public string? SongName => _media?.Meta(LibVLCSharp.Shared.MetadataType.Title);
    public long SongLength { get; private set; }
}