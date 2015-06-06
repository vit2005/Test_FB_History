using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace TestFacebookSDK
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoginTextBox.Text = Settings.login;
            PasswordTextBox.Text = Settings.password;
            FolderTextBox.Text = Settings.folder;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.login = LoginTextBox.Text;
            Settings.password = PasswordTextBox.Text;
            Settings.folder = FolderTextBox.Text;
            Settings.Save();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            FolderTextBox.Text = fbd.SelectedPath;
        }
    }
}
