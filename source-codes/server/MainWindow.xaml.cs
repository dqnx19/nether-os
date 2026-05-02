using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace ServerApp
{
    public partial class MainWindow : Window
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private bool _isRunning;

        private bool _httpIsActive = true;
        private string _httpPort = "80";
        private bool _ftpIsActive;
        private string _ftpPort = "21";
        private bool _sftpIsActive;
        private string _sftpPort = "22";

        public bool HttpIsActive
        {
            get => _httpIsActive;
            set { _httpIsActive = value; OnPropertyChanged(); }
        }
        public string HttpPort
        {
            get => _httpPort;
            set { _httpPort = value; OnPropertyChanged(); }
        }
        public bool FtpIsActive
        {
            get => _ftpIsActive;
            set { _ftpIsActive = value; OnPropertyChanged(); }
        }
        public string FtpPort
        {
            get => _ftpPort;
            set { _ftpPort = value; OnPropertyChanged(); }
        }
        public bool SftpIsActive
        {
            get => _sftpIsActive;
            set { _sftpIsActive = value; OnPropertyChanged(); }
        }
        public string SftpPort
        {
            get => _sftpPort;
            set { _sftpPort = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Prime the backing properties from the initial UI values.
            HttpPort = HttpPortInput.Text;
            FtpPort = FtpPortInput.Text;
            SftpPort = SftpPortInput.Text;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog { Title = "Select Directory" };
            if (dialog.ShowDialog() == true)
            {
                DirectoryPath.Text = dialog.FolderName;
            }
        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            string portText = textBox.Text;
            TextBlock? errorText = null;

            // Keep clean backing properties (HttpPort/FtpPort/SftpPort) in sync.
            switch (textBox.Name)
            {
                case "HttpPortInput":
                    HttpPort = portText;
                    errorText = HttpError;
                    break;
                case "FtpPortInput":
                    FtpPort = portText;
                    errorText = FtpError;
                    break;
                case "SftpPortInput":
                    SftpPort = portText;
                    errorText = SftpError;
                    break;
            }

            if (string.IsNullOrWhiteSpace(portText))
            {
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
                if (errorText != null)
                {
                    errorText.Text = "Port is required";
                    errorText.Visibility = Visibility.Visible;
                }
                return;
            }

            if (!int.TryParse(portText, out int port))
            {
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
                if (errorText != null)
                {
                    errorText.Text = "Must be a number";
                    errorText.Visibility = Visibility.Visible;
                }
                return;
            }

            if (port < 1 || port > 65535)
            {
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e74c3c"));
                if (errorText != null)
                {
                    errorText.Text = "Port must be 1-65535";
                    errorText.Visibility = Visibility.Visible;
                }
                return;
            }

            // Clear the local BorderBrush so the style trigger controls its normal/focused appearance.
            textBox.ClearValue(TextBox.BorderBrushProperty);
            if (errorText != null) errorText.Visibility = Visibility.Collapsed;
        }

        private void PortInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text)) return;

            // Only allow digits in the port input.
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void PortInput_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText, true)) return;

            var text = e.DataObject.GetData(DataFormats.UnicodeText, true) as string;
            if (string.IsNullOrEmpty(text)) return;

            // Block pastes that contain non-digits (digits only are allowed).
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    e.CancelCommand();
                    return;
                }
            }
        }

        private int? ValidatePort(string portText, string protocolName)
        {
            if (string.IsNullOrWhiteSpace(portText))
            {
                MessageBox.Show($"{protocolName}: Port cannot be empty.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (!int.TryParse(portText, out int port))
            {
                MessageBox.Show($"{protocolName}: Port must be a number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (port < 1 || port > 65535)
            {
                MessageBox.Show($"{protocolName}: Port must be between 1 and 65535.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            return port;
        }

        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            string directory = DirectoryPath.Text;
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                MessageBox.Show("Please select a valid directory.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool httpEnabled = HttpEnabled.IsChecked == true;
            bool ftpEnabled = FtpEnabled.IsChecked == true;
            bool sftpEnabled = SftpEnabled.IsChecked == true;

            if (!httpEnabled && !ftpEnabled && !sftpEnabled)
            {
                MessageBox.Show("Please enable at least one protocol.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (httpEnabled)
            {
                int? httpPortNum = ValidatePort(HttpPort, "HTTP");
                if (httpPortNum == null) return;

                string url = $"http://localhost:{httpPortNum}/";
                try
                {
                    _listener = new HttpListener();
                    _listener.Prefixes.Add(url);
                    _listener.Start();
                    _isRunning = true;
                    _cts = new CancellationTokenSource();

                    LaunchBtn.IsEnabled = false;
                    StopBtn.IsEnabled = true;
                    Title = $"NetherOS Server - {url}";
                    StatusText.Text = $"HTTP running on port {httpPortNum}";

                    _ = Task.Run(() => ServeAsync(directory, url));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start HTTP server: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                StatusText.Text = "No protocol enabled";
            }
        }

        private async Task ServeAsync(string directory, string url)
        {
            while (_isRunning && _listener?.IsListening == true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context, directory));
                }
                catch { break; }
            }
        }

        private void HandleRequest(HttpListenerContext context, string directory)
        {
            try
            {
                string localPath = context.Request.Url?.LocalPath ?? "/";
                string filePath = localPath.TrimStart('/');
                if (string.IsNullOrEmpty(filePath) || filePath == "/")
                    filePath = "index.html";

                string fullPath = Path.Combine(directory, filePath);
                var response = context.Response;

                if (File.Exists(fullPath))
                {
                    byte[] content = File.ReadAllBytes(fullPath);
                    response.ContentType = GetMimeType(fullPath);
                    response.ContentLength64 = content.Length;
                    response.OutputStream.Write(content, 0, content.Length);
                }
                else
                {
                    response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("404 Not Found");
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                response.Close();
            }
            catch { }
        }

        private string GetMimeType(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext switch
            {
                ".html" => "text/html",
                ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = false;
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
            _listener = null;

            LaunchBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
            Title = "NetherOS Server";
            StatusText.Text = "";
        }
    }
}