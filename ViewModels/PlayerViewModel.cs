using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;

namespace MusicPlayer.ViewModels;

public class PlayerViewModel : ViewModelBase
{
    public ObservableCollection<string> MusicListings { get; set; } = FileReaderService.GetAll();
    
    private int _currentPtr = 0;
    public string Current => MusicListings[_currentPtr];

    public bool IsPlaying { get; set; }
    
    public double SongProgress { get; set; }

    private double _volumeValue;

    public double VolumeValue
    {
        get => _volumeValue;
        set => SetProperty(ref _volumeValue, Math.Round(value));
    }
    
    #region Comms
    private ICommand? _togglePlayCommand;
    public ICommand TogglePlayCommand => _togglePlayCommand ??= new RelayCommand(TogglePlay);
    private void TogglePlay()
    {
        this.IsPlaying = !this.IsPlaying;
        OnPropertyChanged(nameof(this.IsPlaying));
    }

    private ICommand? _forwardCommand;
    public ICommand ForwardCommand => _forwardCommand ??= new RelayCommand(() =>
    {
        if (_currentPtr < MusicListings.Count - 1) ++_currentPtr;
        OnPropertyChanged(nameof(Current));
    });
    
    private ICommand? _reverseCommand;
    public ICommand ReverseCommand => _reverseCommand ??= new RelayCommand(() =>
    {
        if (_currentPtr > 0) --_currentPtr;
        OnPropertyChanged(nameof(Current));
    });
    #endregion

    public PlayerViewModel()
    {
    }
}