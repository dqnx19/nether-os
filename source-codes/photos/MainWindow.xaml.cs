using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace PhotoViewerApp
{
    public partial class MainWindow : Window
    {
        private readonly List<string> _images = new();
        private int _currentIndex = -1;
        private static readonly string[] SupportedExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp"];

        public MainWindow()
        {
            InitializeComponent();
            UpdateUiState();
        }

        private void OpenImages_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new()
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|All files|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
                return;

            LoadImageList(dialog.FileNames);
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            using Forms.FolderBrowserDialog dialog = new();
            if (dialog.ShowDialog() != Forms.DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
                return;

            string[] files = Directory.GetFiles(dialog.SelectedPath)
                .Where(IsSupportedImage)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (files.Length == 0)
            {
                System.Windows.MessageBox.Show("No supported images found in selected folder.", "Photo Viewer", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            LoadImageList(files);
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0)
                return;

            _currentIndex = (_currentIndex - 1 + _images.Count) % _images.Count;
            ShowCurrentImage();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0)
                return;

            _currentIndex = (_currentIndex + 1) % _images.Count;
            ShowCurrentImage();
        }

        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = 1;
            ImageScrollViewer.ScrollToHorizontalOffset(0);
            ImageScrollViewer.ScrollToVerticalOffset(0);
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            ZoomSlider.Value = 1;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ImageScaleTransform.ScaleX = e.NewValue;
            ImageScaleTransform.ScaleY = e.NewValue;
            ZoomText.Text = $"Zoom: {(int)(e.NewValue * 100)}%";
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    Prev_Click(sender, e);
                    break;
                case Key.Right:
                    Next_Click(sender, e);
                    break;
                case Key.Add:
                case Key.OemPlus:
                    ZoomSlider.Value = Math.Min(ZoomSlider.Maximum, ZoomSlider.Value + 0.1);
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    ZoomSlider.Value = Math.Max(ZoomSlider.Minimum, ZoomSlider.Value - 0.1);
                    break;
            }
        }

        private void LoadImageList(IEnumerable<string> paths)
        {
            _images.Clear();
            _images.AddRange(paths.Where(IsSupportedImage).Distinct(StringComparer.OrdinalIgnoreCase));
            _currentIndex = _images.Count > 0 ? 0 : -1;
            ShowCurrentImage();
        }

        private void ShowCurrentImage()
        {
            if (_currentIndex < 0 || _currentIndex >= _images.Count)
            {
                PreviewImage.Source = null;
                ImageInfoText.Text = "No image loaded";
                UpdateUiState();
                return;
            }

            string filePath = _images[_currentIndex];

            try
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                bitmap.Freeze();

                PreviewImage.Source = bitmap;
                ImageInfoText.Text = Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                PreviewImage.Source = null;
                ImageInfoText.Text = "Failed to load image";
                System.Windows.MessageBox.Show(ex.Message, "Photo Viewer", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            IndexText.Text = $"{_currentIndex + 1} / {_images.Count}";
            UpdateUiState();
        }

        private void UpdateUiState()
        {
            if (_images.Count == 0)
                IndexText.Text = "0 / 0";
        }

        private static bool IsSupportedImage(string path)
        {
            string extension = Path.GetExtension(path);
            return SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
