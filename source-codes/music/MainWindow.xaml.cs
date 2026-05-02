using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MusicPlayerApp
{
    public partial class MainWindow : Window
    {
        private readonly List<string> _tracks = new();
        private readonly DispatcherTimer _positionTimer = new() { Interval = TimeSpan.FromMilliseconds(250) };
        private bool _isDraggingSeek;
        private bool _isPlaying;

        public MainWindow()
        {
            InitializeComponent();
            _positionTimer.Tick += PositionTimer_Tick;
            Player.Volume = VolumeSlider.Value;
        }

        private void AddSongs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new()
            {
                Filter = "Audio files|*.mp3;*.wav;*.flac;*.ogg;*.m4a;*.aac|All files|*.*",
                Multiselect = true
            };

            if (fileDialog.ShowDialog() != true)
                return;

            foreach (string filePath in fileDialog.FileNames)
            {
                if (_tracks.Contains(filePath, StringComparer.OrdinalIgnoreCase))
                    continue;

                _tracks.Add(filePath);
                PlaylistList.Items.Add(Path.GetFileName(filePath));
            }

            if (PlaylistList.SelectedIndex < 0 && PlaylistList.Items.Count > 0)
                PlaylistList.SelectedIndex = 0;
        }

        private void PlaylistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistList.SelectedIndex < 0 || PlaylistList.SelectedIndex >= _tracks.Count)
                return;

            LoadTrack(_tracks[PlaylistList.SelectedIndex]);
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source is null && _tracks.Count > 0)
            {
                if (PlaylistList.SelectedIndex < 0)
                    PlaylistList.SelectedIndex = 0;
                else
                    LoadTrack(_tracks[PlaylistList.SelectedIndex]);
            }

            if (Player.Source is null)
                return;

            if (_isPlaying)
            {
                Player.Pause();
                _isPlaying = false;
                PlayPauseButton.Content = "Play";
                StatusText.Text = "Paused";
                _positionTimer.Stop();
                return;
            }

            Player.Play();
            _isPlaying = true;
            PlayPauseButton.Content = "Pause";
            StatusText.Text = "Playing";
            _positionTimer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source is null)
                return;

            Player.Stop();
            _isPlaying = false;
            PlayPauseButton.Content = "Play";
            PositionSlider.Value = 0;
            CurrentTimeText.Text = "00:00";
            StatusText.Text = "Stopped";
            _positionTimer.Stop();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Player is not null)
                Player.Volume = e.NewValue;
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!Player.NaturalDuration.HasTimeSpan)
                return;

            TimeSpan duration = Player.NaturalDuration.TimeSpan;
            PositionSlider.Maximum = duration.TotalSeconds;
            TotalTimeText.Text = FormatTime(duration);
            CurrentTimeText.Text = "00:00";
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            _positionTimer.Stop();

            if (PlaylistList.SelectedIndex >= 0 && PlaylistList.SelectedIndex < _tracks.Count - 1)
            {
                PlaylistList.SelectedIndex++;
                Player.Play();
                _isPlaying = true;
                PlayPauseButton.Content = "Pause";
                StatusText.Text = "Playing";
                _positionTimer.Start();
                return;
            }

            _isPlaying = false;
            PlayPauseButton.Content = "Play";
            StatusText.Text = "Finished";
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            _isPlaying = false;
            _positionTimer.Stop();
            PlayPauseButton.Content = "Play";
            StatusText.Text = "Cannot play selected file.";
            MessageBox.Show($"Playback failed: {e.ErrorException?.Message}", "Music Player", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void PositionTimer_Tick(object? sender, EventArgs e)
        {
            if (_isDraggingSeek || !Player.NaturalDuration.HasTimeSpan)
                return;

            PositionSlider.Value = Player.Position.TotalSeconds;
            CurrentTimeText.Text = FormatTime(Player.Position);
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PositionSlider.IsMouseCaptureWithin)
                _isDraggingSeek = true;
        }

        private void PositionSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!Player.NaturalDuration.HasTimeSpan)
                return;

            _isDraggingSeek = false;
            Player.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            CurrentTimeText.Text = FormatTime(Player.Position);
        }

        private void LoadTrack(string trackPath)
        {
            try
            {
                Player.Stop();
                Player.Source = new Uri(trackPath);
                SongTitleText.Text = Path.GetFileName(trackPath);
                StatusText.Text = "Ready";
                PositionSlider.Value = 0;
                CurrentTimeText.Text = "00:00";
                TotalTimeText.Text = "00:00";
                _isPlaying = false;
                PlayPauseButton.Content = "Play";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Music Player", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string FormatTime(TimeSpan value)
        {
            if (value.TotalHours >= 1)
                return $"{(int)value.TotalHours:00}:{value.Minutes:00}:{value.Seconds:00}";
            return $"{value.Minutes:00}:{value.Seconds:00}";
        }
    }
}
