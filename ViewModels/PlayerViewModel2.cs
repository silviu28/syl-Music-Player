namespace MusicPlayer.ViewModels;

public partial class PlayerViewModel
{
    public string? SongName => _audioReader?.FileName;
    public long? SongLength => _audioReader?.Length;
}