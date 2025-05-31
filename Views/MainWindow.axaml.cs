using Avalonia.Controls;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.DataContext = new PlayerViewModel();
        InitializeComponent();
    }
}