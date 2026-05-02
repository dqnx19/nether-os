using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;

namespace Explorer
{
    public partial class ZipContentsWindow : Window
    {
        private readonly string _zipPath;
        private readonly List<FileItem> _zipEntries;

        public ZipContentsWindow(string zipPath)
        {
            InitializeComponent();
            _zipPath = zipPath;
            _zipEntries = new List<FileItem>();
            LoadZipContents();
        }


        private void LoadZipContents()
        {
            try
            {
                ZipFileNameText.Text = Path.GetFileName(_zipPath);
                ZipFilePathText.Text = _zipPath;

                using var archive = ZipFile.OpenRead(_zipPath);
                foreach (var entry in archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
                {
                    _zipEntries.Add(new FileItem
                    {
                        Name = entry.Name.Contains('/') || entry.Name.Contains('\\') ? Path.GetFileName(entry.FullName) : entry.Name,
                        FullPath = entry.FullName, // Relative path in ZIP
                        Size = FormatSize(entry.Length),
                        CompressedSize = FormatSize((long)entry.CompressedLength),
                        IsFolder = false // ZIP entries are files; folders are implied by paths
                    });
                }

                ZipContentsList.ItemsSource = _zipEntries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ZIP: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void ExtractAll_Click(object sender, RoutedEventArgs e)
        {
            var extractPath = Path.Combine(Path.GetDirectoryName(_zipPath)!, Path.GetFileNameWithoutExtension(_zipPath));
            ExtractEntries(_zipEntries, extractPath);
        }

        private void ExtractSelected_Click(object sender, RoutedEventArgs e)
        {
            if (ZipContentsList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select items to extract.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectedEntries = ZipContentsList.SelectedItems.Cast<FileItem>().ToList();
            var extractPath = Path.Combine(Path.GetDirectoryName(_zipPath)!, Path.GetFileNameWithoutExtension(_zipPath));
            ExtractEntries(selectedEntries, extractPath);
        }

        private void ExtractEntries(List<FileItem> entries, string baseExtractPath)
        {
            try
            {
                using var archive = ZipFile.OpenRead(_zipPath);
                Directory.CreateDirectory(baseExtractPath);

                foreach (var entryItem in entries)
                {
                    var entry = archive.GetEntry(entryItem.FullPath);
                    if (entry != null)
                    {
                        var targetPath = Path.Combine(baseExtractPath, entryItem.FullPath.Replace('/', Path.DirectorySeparatorChar));
                        var targetDir = Path.GetDirectoryName(targetPath);
                        if (!string.IsNullOrEmpty(targetDir))
                            Directory.CreateDirectory(targetDir);

                        entry.ExtractToFile(targetPath, overwrite: false);
                    }
                }

                MessageBox.Show($"Extracted to: {baseExtractPath}", "Extract Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Extract error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static string FormatSize(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSize = bytes;
            while (dblSize >= 1024 && i < Suffix.Length - 1)
            {
                dblSize /= 1024;
                i++;
            }
            return $"{dblSize:0.##} {Suffix[i]}";
        }
    }

    // Extend FileItem for ZIP (add to MainWindow if needed, but here for completeness)
    public class ZipFileItem : FileItem
    {
        public new string CompressedSize { get; set; } = "";
    }

}

