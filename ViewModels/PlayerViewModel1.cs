using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Views;
using LibVLCSharp;
using LibVLCSharp.Shared;
using Avalonia.Threading;

namespace MusicPlayer.ViewModels;

public partial class PlayerViewModel : ViewModelBase
{
    public ObservableCollection<string> MusicListings { get; set; } = FileReaderService.GetAll();

    private int _currentPtr = 0;
    public string Current => MusicListings.Count > 0 ? MusicListings[_currentPtr] : "none";

    public bool IsPlaying { get; set; }

    public double SongProgress
    {
        get => _player.Position * 100;
        set
        {
            _player.Position = (float)(value / 100);
            OnPropertyChanged(nameof(SongProgress));
        }
    }

    private double _volumeValue = 80;

    public double VolumeValue
    {
        get => _volumeValue;
        set
        {
            SetProperty(ref _volumeValue, Math.Round(value));
            _player.Volume = (int)_volumeValue;
        }
    }

    private LibVLC _VLCinvoke;
    private MediaPlayer _player;
    private Media? _media;

    #region Comms

    private ICommand? _togglePlayCommand;
    public ICommand TogglePlayCommand => _togglePlayCommand ??= new RelayCommand(TogglePlay);

    private void TogglePlay()
    {
        try
        {
            if (!IsPlaying)
            {
                if (_media is null)
                {
                    _media = new(_VLCinvoke, Current, FromType.FromPath);
                    _media.Parse();

                    OnPropertyChanged(nameof(SongName));
                }
                _player.Play(_media);
                _timelineTimer.Start();
            }
            else
            {
                _player.Stop();
                _timelineTimer.Stop();
            }

            IsPlaying = !IsPlaying;
            OnPropertyChanged(nameof(IsPlaying));
        }
        catch (Exception ex)
        {
            PopupModal.Show(ex.Message);
        }
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

    private ICommand? _openFileDialogCommand;
    public ICommand OpenFileDialogCommand => _openFileDialogCommand ??= new RelayCommand(OpenFileDialog);

    private async void OpenFileDialog()
    {
        try
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Select directory"
            };
            var dialogResult =
                await dialog.ShowAsync(
                    App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                        ? desktop.MainWindow
                        : null);

            if (!string.IsNullOrWhiteSpace(dialogResult?[0]))
            {
                FileReaderService.CurrentDirectory = dialogResult[0];
                MusicListings.Clear();
                MusicListings = FileReaderService.GetAll();
                _currentPtr = 0;

                OnPropertyChanged(nameof(MusicListings));
                OnPropertyChanged(nameof(Current));
            }
        }
        catch (Exception ex)
        {
            PopupModal.Show($"Something happened: {ex.Message}");
        }
    }
    #endregion

    private DispatcherTimer _timelineTimer;

    public PlayerViewModel()
    {
        Core.Initialize();
        _VLCinvoke = new("--no-video-title-show", 
                         "--file-caching=300",
                         "--audio-time-stretch");
        _player = new(_VLCinvoke);
        _timelineTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timelineTimer.Tick += MoveTimelineForward;
    }

    private void MoveTimelineForward(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SongProgress));
    }
}