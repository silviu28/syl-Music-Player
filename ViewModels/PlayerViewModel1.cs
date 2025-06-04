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

namespace MusicPlayer.ViewModels;

public partial class PlayerViewModel : ViewModelBase
{
    public ObservableCollection<string> MusicListings { get; set; } = FileReaderService.GetAll();

    private int _currentPtr = 0;
    public string Current => MusicListings.Count > 0 ? MusicListings[_currentPtr] : "none";

    public bool IsPlaying { get; set; }

    public double SongProgress { get; set; }

    private double _volumeValue;

    public double VolumeValue
    {
        get => _volumeValue;
        set => SetProperty(ref _volumeValue, Math.Round(value));
    }

    private LibVLC _VLCinvoke;
    private MediaPlayer _player;

    #region Comms

    private ICommand? _togglePlayCommand;
    public ICommand TogglePlayCommand => _togglePlayCommand ??= new RelayCommand(TogglePlay);

    private void TogglePlay()
    {
        try
        {
            if (!IsPlaying)
            {
                using Media _media = new(_VLCinvoke, Current, FromType.FromPath);
                _player.Play(_media);
            }
            else
                _player.Stop();

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

    public PlayerViewModel()
    {
        Core.Initialize();
        _VLCinvoke = new();
        _player = new(_VLCinvoke);
    }
}