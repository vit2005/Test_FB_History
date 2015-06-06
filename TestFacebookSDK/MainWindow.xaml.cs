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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using Facebook;
using System.Dynamic;

namespace TestFacebookSDK
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FacebookClient fb;
        Model m;
        public MainWindow()
        {
            InitializeComponent();
            m = new Model();
        }

        private void Login()
        {
            dynamic parameters = new ExpandoObject();

            parameters.client_id = "368655543325694";
            parameters.client_secret = "60f157fe2a579010fde58fbb876a46e1";
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";
            parameters.response_type = "token";
            parameters.display = "popup";
            parameters.scope = "public_profile,user_about_me,user_likes,user_birthday,user_hometown,user_status,user_website,read_mailbox,user_photos,user_posts,user_videos";

            Uri loginUri;
            fb = new FacebookClient();
            loginUri = fb.GetLoginUrl(parameters);

            WebBrowserLogin.Navigate(loginUri);
            
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Login();
            EnterData.IsEnabled = true;
            Connect.IsEnabled = false;
        }

        private void EnterData_Click(object sender, RoutedEventArgs e)
        {
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)WebBrowserLogin.Document;
            var login = doc.body.document.getElementById("email");
            login.value = Settings.login;
            var password = doc.body.document.getElementById("pass");
            password.value = Settings.password;
            var submit = doc.body.document.getElementById("u_0_2");
            submit.Click();
        }

        private void WebBrowserLogin_Navigated(object sender, NavigationEventArgs e)
        {
            string url = e.Uri.ToString();
            

            if (url.Contains("access_token"))
            {
                WebBrowserLogin.Visibility = System.Windows.Visibility.Hidden;

                int index = url.IndexOf("access_token=") + 13;
                url = url.Substring(index);
                url = url.Remove(url.IndexOf("&"));
                Settings.accessToken = url;
                ConnectedCheckbox.IsChecked = true;
                EnterData.IsEnabled = false;
                SaveData.IsEnabled = true;
                LogOut.IsEnabled = true;
            }
        }

        private void SettingsClick(Object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            var fb = new FacebookClient(Settings.accessToken);
            System.IO.Directory.CreateDirectory(Settings.folder);

            var person = (IDictionary<string, object>)fb.Get("me");
            m.SavePerson(person);
            PersonSaved.IsChecked = true;

            var links = (IDictionary<string, object>)fb.Get("me/links");
            m.SaveLinks(links);
            LinksSaved.IsChecked = true;

            var messages = (IDictionary<string, object>)fb.Get("me/inbox");
            m.SaveMessages(messages);
            MessagesSaved.IsChecked = true;

            var photos = (IDictionary<string, object>)fb.Get("me/photos/uploaded?fields=album");
            m.SavePhotos(photos);
            PhotosSaved.IsChecked = true;

            var posts = (IDictionary<string, object>)fb.Get("me/feed");
            m.SavePosts(posts);
            PostsSaved.IsChecked = true;

            MessageBox.Show("Done");
            PersonSaved.IsChecked = false;
            LinksSaved.IsChecked = false;
            MessagesSaved.IsChecked = false;
            PhotosSaved.IsChecked = false;
            PostsSaved.IsChecked = false;
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserLogin.Visibility = System.Windows.Visibility.Visible;
            var fb = new FacebookClient();
            var logoutUrl = fb.GetLogoutUrl(new { access_token = Settings.accessToken, next = "https://www.facebook.com/connect/login_success.html" });
            WebBrowserLogin.Navigate(logoutUrl);
            Settings.accessToken = null;
            ConnectedCheckbox.IsChecked = false;
            LogOut.IsEnabled = false;
            SaveData.IsEnabled = false;
            Connect.IsEnabled = true;
        }
    }
}
