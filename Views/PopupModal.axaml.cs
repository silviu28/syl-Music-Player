using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Views;

/// <summary>
/// Equivalent of WPF's MessageBox - a singleton that displays a given message. Does not freeze the main thread.
/// </summary>
public partial class PopupModal : Window
{
    private static PopupModal? _instance;
    private static readonly PopupModalViewModel ViewModel = new();

    private PopupModal()
    {
        InitializeComponent();
        this.DataContext = ViewModel;
    }

    /// <summary>
    /// Display a modal.
    /// </summary>
    /// <param name="message">your message</param>
    public static void Show(string message)
    {
        ViewModel.ModalMessage = message;
        if (_instance is null)
        {
            _instance = new();
            _instance.Show();
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        _instance?.Close();
        _instance = null;
    }
}