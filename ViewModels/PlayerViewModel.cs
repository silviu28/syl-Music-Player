using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels;

public class PlayerViewModel : ViewModelBase
{
    public ObservableCollection<object> MusicListings { get; set; }
}