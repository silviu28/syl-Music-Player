namespace MusicPlayer.Models;

public class Song
{
    public int Length { get; }
    public string? Title { get; }
    public string? Format { get; }

    public Song(int length, string? title, string? format)
    {
        Length = length;
        Title = title;
        Format = format;
    }
}