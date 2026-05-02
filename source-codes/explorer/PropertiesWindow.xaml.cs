using System.Windows;

namespace Explorer
{
    public partial class PropertiesWindow : Window
    {
        public PropertiesWindow(string name, string location, string size, string extension, string fileType, string category, string fullPath, string iconEmoji)
        {
            InitializeComponent();
            
            FileNameText.Text = name;
            FileTypeText.Text = fileType;
            LocationText.Text = location;
            SizeText.Text = size;
            ExtensionText.Text = extension;
            TypeText.Text = fileType;
            CategoryText.Text = category;
            FullPathText.Text = fullPath;
            FileIconEmoji.Text = iconEmoji;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}