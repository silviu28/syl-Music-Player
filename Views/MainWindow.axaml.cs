using Avalonia.Controls;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new PlayerViewModel();
    }
}