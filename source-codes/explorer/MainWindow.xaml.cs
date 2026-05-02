using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Specialized;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Explorer
{
    public class FileTypeCategory
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";
        
        [JsonPropertyName("extensions")]
        public Dictionary<string, string> Extensions { get; set; } = new();
    }

    public static class FileTypeDatabase
    {
        private static Dictionary<string, string>? _types;
        private static Dictionary<string, string>? _categories;

        public static void Load(string jsonPath)
        {
            try
            {
                string json = File.ReadAllText(jsonPath);
                var categories = JsonSerializer.Deserialize<List<FileTypeCategory>>(json);
                
                _types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                _categories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        foreach (var kvp in category.Extensions)
                        {
                            _types[kvp.Key] = kvp.Value;
                            _categories[kvp.Key] = category.Title;
                        }
                    }
                }
            }
            catch
            {
                _types = new Dictionary<string, string>();
                _categories = new Dictionary<string, string>();
            }
        }

        public static string GetType(string extension)
        {
            if (_types is null || string.IsNullOrEmpty(extension)) return "File";
            return _types.TryGetValue(extension, out var type) ? type : "File";
        }

        public static string GetCategory(string extension)
        {
            if (_categories is null || string.IsNullOrEmpty(extension)) return "Unknown";
            return _categories.TryGetValue(extension, out var category) ? category : "Unknown";
        }
    }

    public partial class MainWindow : Window
    {
        private BitmapImage? _folderIcon;
        private BitmapImage? _desktopIcon;
        private BitmapImage? _documentsIcon;
        private BitmapImage? _downloadsIcon;
        private BitmapImage? _musicIcon;
        private BitmapImage? _picturesIcon;
        private BitmapImage? _videosIcon;
        private BitmapImage? _diskIcon;
        private BitmapImage? _systemDiskIcon;

        public MainWindow()
        {
            InitializeComponent();
            LoadFileTypes();
            LoadCustomIcons();
            ShowDrives();
        }

        private void LoadFileTypes()
        {
            string language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string fileName = language switch
            {
                "cs" => "filetypes-cz.json",
                "de" => "filetypes-de.json",
                _ => "filetypes-en.json"
            };
            FileTypeDatabase.Load(fileName);
        }

        private void LoadCustomIcons()
        {
            try
            {
                _folderIcon = new BitmapImage(new Uri("img/folder.png", UriKind.Relative));
                _desktopIcon = new BitmapImage(new Uri("img/desktop.png", UriKind.Relative));
                _documentsIcon = new BitmapImage(new Uri("img/documents.png", UriKind.Relative));
                _downloadsIcon = new BitmapImage(new Uri("img/downloads.png", UriKind.Relative));
                _musicIcon = new BitmapImage(new Uri("img/music.png", UriKind.Relative));
                _picturesIcon = new BitmapImage(new Uri("img/pictures.png", UriKind.Relative));
                _videosIcon = new BitmapImage(new Uri("img/videos.png", UriKind.Relative));
                _diskIcon = new BitmapImage(new Uri("img/disk.png", UriKind.Relative));
                _systemDiskIcon = new BitmapImage(new Uri("img/system-disk.png", UriKind.Relative));
            }
            catch
            {
            }
        }

        private void ShowDrives()
        {
            PathBar.Text = "This PC";
            DrivesControl.Visibility = Visibility.Visible;
            FileListView.Visibility = Visibility.Collapsed;

            List<FileItem> driveItems = new List<FileItem>();
            string systemRoot = Path.GetPathRoot(Environment.SystemDirectory) ?? string.Empty;

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    long total = drive.TotalSize;
                    long used = total - drive.TotalFreeSpace;
                    bool isSystemDrive = string.Equals(drive.Name, systemRoot, StringComparison.OrdinalIgnoreCase);

                    driveItems.Add(new FileItem
                    {
                        Name = $"{drive.VolumeLabel} ({drive.Name.Replace("\\", "")})",
                        FullPath = drive.Name,
                        IconSource = isSystemDrive ? _systemDiskIcon : _diskIcon,
                        IconEmoji = "",
                        UsedPercentage = (total > 0) ? ((double)used / total) * 100 : 0,
                        Size = $"{FormatSize(used)} used of {FormatSize(total)}",
                        IsFolder = false
                    });
                }
            }
            DrivesList.ItemsSource = driveItems;
        }

        private BitmapImage? GetFolderIconForPath(string? folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return _folderIcon;

            string normalizedPath = Path.GetFullPath(folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string desktopPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string downloadsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads").TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string documentsPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string picturesPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string videosPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string musicPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (string.Equals(normalizedPath, desktopPath, StringComparison.OrdinalIgnoreCase))
                return _desktopIcon;
            if (string.Equals(normalizedPath, downloadsPath, StringComparison.OrdinalIgnoreCase))
                return _downloadsIcon;
            if (string.Equals(normalizedPath, documentsPath, StringComparison.OrdinalIgnoreCase))
                return _documentsIcon;
            if (string.Equals(normalizedPath, picturesPath, StringComparison.OrdinalIgnoreCase))
                return _picturesIcon;
            if (string.Equals(normalizedPath, videosPath, StringComparison.OrdinalIgnoreCase))
                return _videosIcon;
            if (string.Equals(normalizedPath, musicPath, StringComparison.OrdinalIgnoreCase))
                return _musicIcon;

            string folderName = Path.GetFileName(normalizedPath);
            if (!string.IsNullOrEmpty(folderName))
            {
                if (string.Equals(folderName, "Desktop", StringComparison.OrdinalIgnoreCase))
                    return _desktopIcon;
                if (string.Equals(folderName, "Downloads", StringComparison.OrdinalIgnoreCase))
                    return _downloadsIcon;
                if (string.Equals(folderName, "Documents", StringComparison.OrdinalIgnoreCase))
                    return _documentsIcon;
                if (string.Equals(folderName, "Pictures", StringComparison.OrdinalIgnoreCase))
                    return _picturesIcon;
                if (string.Equals(folderName, "Videos", StringComparison.OrdinalIgnoreCase))
                    return _videosIcon;
                if (string.Equals(folderName, "Music", StringComparison.OrdinalIgnoreCase))
                    return _musicIcon;
            }

            return _folderIcon;
        }

        private void LoadDirectory(string? path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;
            try
            {
                DrivesControl.Visibility = Visibility.Collapsed;
                FileListView.Visibility = Visibility.Visible;
                PathBar.Text = path;

                List<FileItem> items = new List<FileItem>();
                DirectoryInfo di = new DirectoryInfo(path);

                foreach (var dir in di.GetDirectories())
                    items.Add(new FileItem { Name = dir.Name, FullPath = dir.FullName, Size = "--", FileType = "Folder", IconSource = GetFolderIconForPath(dir.FullName), IsFolder = true });

                foreach (var file in di.GetFiles())
                    items.Add(new FileItem { Name = file.Name, FullPath = file.FullName, Size = FormatSize(file.Length), FileType = GetFileType(file.Extension), IconEmoji = "📄", IsFolder = false });

                FileListView.ItemsSource = items;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                ShowDrives();
            }
        }

        private string FormatSize(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSzie = bytes;
            while (dblSzie >= 1024 && i < Suffix.Length - 1)
            {
                i++;
                dblSzie /= 1024;
            }
            return $"{dblSzie:0.##} {Suffix[i]}";
        }

        private string GetFileType(string extension)
        {
            return FileTypeDatabase.GetType(extension);
        }

        private void Drive_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag != null) LoadDirectory(btn.Tag.ToString());
        }

        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected)
            {
                if (selected.IsFolder) 
                {
                    LoadDirectory(selected.FullPath);
                }
                else
                {
                    string extension = Path.GetExtension(selected.FullPath).ToLowerInvariant();
                    if (extension == ".zip")
                    {
                        var zipWindow = new ZipContentsWindow(selected.FullPath);
                        zipWindow.Owner = this;
                        zipWindow.ShowDialog();
                    }
                    else
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo { FileName = selected.FullPath, UseShellExecute = true });
                        }
                        catch { }
                    }
                }
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e) => FileListView_MouseDoubleClick(sender, null!);

        private void ExtractHere_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected && Path.GetExtension(selected.FullPath).ToLowerInvariant() == ".zip")
            {
                try
                {
                    string extractPath = Path.Combine(PathBar.Text, Path.GetFileNameWithoutExtension(selected.Name));
                    using var archive = ZipFile.OpenRead(selected.FullPath);
                    Directory.CreateDirectory(extractPath);
                    foreach (var entry in archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
                    {
                        var targetPath = Path.Combine(extractPath, entry.FullName.Replace('/', Path.DirectorySeparatorChar));
                        var targetDir = Path.GetDirectoryName(targetPath);
                        if (!string.IsNullOrEmpty(targetDir)) Directory.CreateDirectory(targetDir);
                        entry.ExtractToFile(targetPath, overwrite: false);
                    }
                    MessageBox.Show($"Extracted to {extractPath}", "Extract Complete");
                    LoadDirectory(PathBar.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Extract error: {ex.Message}");
                }
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected)
            {
                DataObject dataObject = new DataObject();
                StringCollection files = new StringCollection { selected.FullPath };
                dataObject.SetFileDropList(files);
                dataObject.SetData("Preferred DropEffect", new MemoryStream(new byte[] { 2, 0, 0, 0 }));
                System.Windows.Clipboard.SetDataObject(dataObject, true);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected)
            {
                System.Windows.Clipboard.SetFileDropList(new StringCollection { selected.FullPath });
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IDataObject? dataObject = System.Windows.Clipboard.GetDataObject();
                if (dataObject is null) return;

                if (!dataObject.GetDataPresent(System.Windows.DataFormats.FileDrop)) return;

                string[]? files = dataObject.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (files is null || files.Length == 0) return;

                string currentPath = PathBar.Text;
                if (string.IsNullOrEmpty(currentPath) || !Directory.Exists(currentPath)) return;

                bool isCut = false;
                if (dataObject.GetDataPresent("Preferred DropEffect"))
                {
                    object? rawEffect = dataObject.GetData("Preferred DropEffect");
                    if (rawEffect is MemoryStream ms)
                    {
                        byte[] buffer = new byte[4];
                        ms.Read(buffer, 0, 4);
                        // DROPEFFECT_MOVE == 2
                        isCut = BitConverter.ToInt32(buffer, 0) == 2;
                    }
                    else if (rawEffect is byte[] bytes && bytes.Length >= 4)
                    {
                        isCut = BitConverter.ToInt32(bytes, 0) == 2;
                    }
                }

                foreach (string sourcePath in files)
                {
                    string fileName = Path.GetFileName(sourcePath);
                    string uniqueTarget = GetUniquePath(currentPath, fileName, "");

                    if (isCut)
                    {
                        if (Directory.Exists(sourcePath))
                        {
                            Directory.Move(sourcePath, uniqueTarget);
                        }
                        else if (File.Exists(sourcePath))
                        {
                            File.Move(sourcePath, uniqueTarget);
                        }
                    }
                    else
                    {
                        if (Directory.Exists(sourcePath))
                        {
                            CopyDirectory(sourcePath, uniqueTarget);
                        }
                        else if (File.Exists(sourcePath))
                        {
                            File.Copy(sourcePath, uniqueTarget);
                        }
                    }
                }

                if (isCut) System.Windows.Clipboard.Clear();
                LoadDirectory(currentPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestDir = Path.Combine(destDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestDir);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem s)
            {
                try
                {
                    if (s.IsFolder) FileSystem.DeleteDirectory(s.FullPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    else FileSystem.DeleteFile(s.FullPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                    LoadDirectory(PathBar.Text);
                }
                catch
                {
                }
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected)
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("Rename to:", "Rename", selected.Name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    try
                    {
                        string target = Path.Combine(Path.GetDirectoryName(selected.FullPath)!, newName);
                        if (selected.IsFolder) Directory.Move(selected.FullPath, target);
                        else File.Move(selected.FullPath, target);
                        LoadDirectory(PathBar.Text);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void Properties_Click(object sender, RoutedEventArgs e)
        {
            if (FileListView.SelectedItem is FileItem selected)
            {
                string extension = selected.IsFolder ? "" : Path.GetExtension(selected.FullPath);
                string fileType = selected.IsFolder ? "Folder" : FileTypeDatabase.GetType(extension);
                string category = selected.IsFolder ? "Folder" : FileTypeDatabase.GetCategory(extension);
                string location = Path.GetDirectoryName(selected.FullPath) ?? "";
                string iconEmoji = selected.IsFolder ? "📁" : "📄";

                var propertiesWindow = new PropertiesWindow(
                    selected.Name,
                    location,
                    selected.Size,
                    extension,
                    fileType,
                    category,
                    selected.FullPath,
                    iconEmoji
                );
                propertiesWindow.Owner = this;
                propertiesWindow.ShowDialog();
            }
        }

        private void NewFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(PathBar.Text))
                    return;

                string targetPath = GetUniquePath(PathBar.Text, "New Folder", "");
                Directory.CreateDirectory(targetPath);
                LoadDirectory(PathBar.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void NewTextFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(PathBar.Text))
                    return;

                string targetPath = GetUniquePath(PathBar.Text, "New Text Document", ".txt");
                using FileStream fs = File.Create(targetPath);
                LoadDirectory(PathBar.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void NewZipFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(PathBar.Text))
                    return;

                string targetPath = GetUniquePath(PathBar.Text, "Compressed Folder", ".zip");
                using FileStream fs = File.Create(targetPath);
                using ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create);
                LoadDirectory(PathBar.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private static string GetUniquePath(string directoryPath, string baseName, string extension)
        {
            string fileName = $"{baseName}{extension}";
            string candidatePath = Path.Combine(directoryPath, fileName);
            int suffix = 1;

            while (Directory.Exists(candidatePath) || File.Exists(candidatePath))
            {
                fileName = $"{baseName} ({suffix}){extension}";
                candidatePath = Path.Combine(directoryPath, fileName);
                suffix++;
            }

            return candidatePath;
        }

        private void FileContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            FileItem? selectedItem = FileListView.SelectedItem as FileItem;
            bool hasSelection = selectedItem != null;
            bool canCreateNew = Directory.Exists(PathBar.Text);


            OpenMenuItem.IsEnabled = hasSelection;
            CutMenuItem.IsEnabled = hasSelection;
            CopyMenuItem.IsEnabled = hasSelection;
            RenameMenuItem.IsEnabled = hasSelection;
            DeleteMenuItem.IsEnabled = hasSelection;
            PropertiesMenuItem.IsEnabled = hasSelection;

            NewFolderMenuItem.IsEnabled = canCreateNew;
            NewTextFileMenuItem.IsEnabled = canCreateNew;
            NewZipFileMenuItem.IsEnabled = canCreateNew;

            bool hasClipboardData = false;
            try
            {
                IDataObject? dataObject = System.Windows.Clipboard.GetDataObject();
                hasClipboardData = dataObject is not null && dataObject.GetDataPresent(System.Windows.DataFormats.FileDrop);
            }
            catch { }
            PasteMenuItem.IsEnabled = hasClipboardData && canCreateNew;

            // ZIP specific
            if (selectedItem != null)
            {
                ExtractHereMenuItem.IsEnabled = Path.GetExtension(selectedItem.FullPath).ToLowerInvariant() == ".zip";
            }
            else
            {
                ExtractHereMenuItem.IsEnabled = false;
            }

        }


        private void FileListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject? source = e.OriginalSource as DependencyObject;
            ListViewItem? clickedItem = FindAncestor<ListViewItem>(source);

            if (clickedItem is null)
            {
                FileListView.SelectedItem = null;
                return;
            }

            clickedItem.IsSelected = true;
        }

        private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current is not null)
            {
                if (current is T typed)
                    return typed;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private void NavigateToHome(object sender, RoutedEventArgs e) => ShowDrives();
        private void NavigateToDesktop(object sender, RoutedEventArgs e) => LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        private void NavigateToDownloads(object sender, RoutedEventArgs e) => LoadDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
        private void NavigateToDocuments(object sender, RoutedEventArgs e) => LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        private void NavigateToPictures(object sender, RoutedEventArgs e) => LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        private void NavigateToVideos(object sender, RoutedEventArgs e) => LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
        private void NavigateToMusic(object sender, RoutedEventArgs e) => LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

        private void GoUp_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(PathBar.Text))
            {
                var parent = Directory.GetParent(PathBar.Text);
                if (parent != null)
                    LoadDirectory(parent.FullName);
            }
            else
            {
                ShowDrives();
            }
        }

        private void PathBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string path = PathBar.Text.Trim();
                if (Directory.Exists(path))
                    LoadDirectory(path);
                else if (path.Equals("this pc", StringComparison.OrdinalIgnoreCase))
                    ShowDrives();
            }
        }
    }

    public class FileItem
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public string Size { get; set; } = "";
        public string FileType { get; set; } = "";
        public string IconEmoji { get; set; } = "";
        public ImageSource? IconSource { get; set; }
        public bool IsFolder { get; set; }
        public double UsedPercentage { get; set; }
        public string CompressedSize { get; set; } = "";
    }
}
