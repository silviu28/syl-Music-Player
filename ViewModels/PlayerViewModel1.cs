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
using NAudio.Wave;

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

    private void OnButtonPlay(object? sender, EventArgs e)
    {
        if (_audioOutput is null)
        {
            _audioOutput = new();
            _audioOutput.PlaybackStopped += OnPlaybackStop;
        }

        if (_audioReader is null)
        {
            _audioReader = new(@".");
            _audioOutput.Init(_audioReader);
        }

        _audioOutput.Play();
    }

    private void OnPlaybackStop(object? sender, EventArgs e)
    {
        _audioOutput?.Dispose();
        _audioOutput = null;
        _audioReader?.Dispose();
        _audioReader = null;
    }


    private WaveOutEvent? _audioOutput;

    public WaveOutEvent AudioOutput
    {
        get
        {
            if (_audioOutput is null)
            {
                _audioOutput = new WaveOutEvent();
                _audioOutput.PlaybackStopped += (_, _) =>
                {
                    AudioOutput.Dispose();
                    _audioOutput = null;
                    AudioReader.Dispose();
                    _audioReader = null;
                };
            }

            return _audioOutput;
        }
    }

    private AudioFileReader? _audioReader;
    public AudioFileReader AudioReader
    {
        get
        {
            if (_audioReader is null)
            {
                _audioReader = new(Current);
                AudioOutput.Init(_audioReader);
            }
            return _audioReader;
        }
    }

    #region Comms

    private ICommand? _togglePlayCommand;
    public ICommand TogglePlayCommand => _togglePlayCommand ??= new RelayCommand(TogglePlay);

    private void TogglePlay()
    {
        IsPlaying = !IsPlaying;
        // if (!IsPlaying)
        //     AudioOutput.Stop();
        // else
        //     AudioOutput.Play();
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
    }
}