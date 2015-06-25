using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Facebook;
using System.Dynamic;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace NewFacebookHistory
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FacebookClient fb;
        ISocialNetwork m;
        bool tempFileIsUsing = false;

        List<Thread> threads;
        public MainWindow()
        {
            InitializeComponent();
            threads = new List<Thread>();
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = (System.Drawing.Icon)Properties.Resources.facebook_icon_flat;
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            SetBackgroundImage();
            
        }

        private void SetBackgroundImage(string s = "")
        {
            if (s == "")
                return;

            //string s = @"E:\pic\y_91a78b7b.jpg";
            System.Drawing.Image OriginalPhoto = System.Drawing.Image.FromFile(s);
            System.Drawing.Image imageBackground = System.Drawing.Image.FromFile("background2.jpg");
            int w = OriginalPhoto.Width;
            int h = OriginalPhoto.Height;
            double scalew, scaleh;
            if (w > h) {
                scalew = 1;
                scaleh = (double)h / (double)w;
            } else {
                scaleh = 1;
                scalew = (double)w / (double)h;
            }
            Bitmap ResizedPhoto = new Bitmap(OriginalPhoto, (int)(450 * scalew), (int)(450 * scaleh));


            System.Drawing.Image img = new Bitmap(imageBackground.Width, imageBackground.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                int height = 450, width = 450;
                height = (int)MainGrid.ActualHeight;
                width = (int)MainGrid.ActualWidth;
                gr.DrawImage(imageBackground, new System.Drawing.Point(0, 0));
                if (scalew == 1)
                {
                    gr.DrawImage((System.Drawing.Image)ResizedPhoto, new System.Drawing.Point(0, (int)(height/2)+24 - ResizedPhoto.Height / 2));
                }
                else
                {
                    gr.DrawImage((System.Drawing.Image)ResizedPhoto, new System.Drawing.Point((int)(width/2) - ResizedPhoto.Width / 2, 24));
                }

            }
            ResizedPhoto = new Bitmap(img);

            string filename;
            if (tempFileIsUsing)
            {
                filename = "temp2.png";
                tempFileIsUsing = false;
                File.Delete(System.IO.Path.GetTempPath() + "temp2.png");
            }
            else
            {
                filename = "temp.png";
                tempFileIsUsing = true;
                File.Delete(System.IO.Path.GetTempPath() + "temp.png");
            }

            using (FileStream stream = new FileStream(System.IO.Path.GetTempPath() + filename, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      ResizedPhoto.GetHbitmap(),
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
    
                encoder.Frames.Add(BitmapFrame.Create(bs));
                encoder.Save(stream);
            }

            var source = new BitmapImage();
            using (var fs = new FileStream(System.IO.Path.GetTempPath() + filename, FileMode.Open))
            {
                source.BeginInit();
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.StreamSource = fs;
                source.EndInit();
            }
            source.Freeze();

            Dispatcher.Invoke(() => MainGrid.Background = new ImageBrush(source));
        }

        private void StartAnimation()
        {
            //DoubleAnimation da = new DoubleAnimation();
            //da.From = 0;
            //da.To = 100;
            //da.Duration = TimeSpan.FromSeconds(100);
            //da.Completed += EndSetAnimation;
            //try {
            //    Dispatcher.Invoke(() => MainGrid.Background.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, da));
            //} catch (Exception) {
            //    EndSetAnimation(null,null);
            //}
            for (int i = 0; i < 100; i++)
            {
                MainGrid.Dispatcher.Invoke(() => MainGrid.Background.Opacity = i, DispatcherPriority.Normal);
                MainGrid.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
                Thread.Sleep(200);
            }
            EndSetAnimation(null, null);
        }

        private void EndAnimation()
        {
            //DoubleAnimation da = new DoubleAnimation();
            //da.From = 100;
            //da.To = 0;
            //da.Duration = TimeSpan.FromSeconds(100);
            //da.Completed += StartSetbackground;
            //try {
            //    Dispatcher.Invoke(() => MainGrid.Background.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, da));
            //} catch (Exception) {
            //    StartSetbackground(null, null);
            //}
            for (int i = 100; i < 0; i--)
            {
                MainGrid.Dispatcher.Invoke(() => MainGrid.Background.Opacity = i, DispatcherPriority.Normal);
                MainGrid.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
                Thread.Sleep(200);
            }
            StartSetbackground(null, null);
        }

        private void WarpedStartSetBackground()
        {
            StartSetbackground(null, null);
        }

        private void StartSetbackground(object sender, EventArgs e)
        {
            bool flag = true;
            while (flag)
            {
                //bool isNull = true;
                //Dispatcher.Invoke(() => isNull = ISocialNetwork.PhotosDatabase == null);
                //if (!isNull)
                //{
                //    bool hasPictures = false;
                //    Dispatcher.Invoke(() => hasPictures = ISocialNetwork.PhotosDatabase.Count != 0);
                //    if (hasPictures)
                //    {
                if (ISocialNetwork.PhotosDatabase != null && ISocialNetwork.PhotosDatabase.Count != 0)
                {
                    Random r = new Random();
                    SetBackgroundImage(ISocialNetwork.PhotosDatabase[r.Next(ISocialNetwork.PhotosDatabase.Count)].uri);
                    StartAnimation();
                    flag = false;
                    return;
                }
                        
                    //}
                //}

                Thread.Sleep(2000);
                flag = true;
            }
            
        }

        private void EndSetAnimation(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            EndAnimation();
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

        private void MakeVideoClick(Object sender, MouseButtonEventArgs e)
        {
            MakeVideoWindow makeVideoWindow = new MakeVideoWindow();
            makeVideoWindow.ShowDialog();
        }

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            m = new FacebookSN();
            System.IO.Directory.CreateDirectory(Settings.folder);

            Thread person_thread = new Thread(m.Person);
            person_thread.Start();

            Thread links_thread = new Thread(m.Links);
            links_thread.Start();

            Thread messages_thread = new Thread(m.Messages);
            messages_thread.Start();

            Thread photos_thread = new Thread(m.Photos);
            photos_thread.Start();

            Thread posts_thread = new Thread(m.Posts);
            posts_thread.Start();

            Thread progressbar_thread = new Thread(ProgressController);
            progressbar_thread.Start();

            threads.Add(person_thread);
            threads.Add(links_thread);
            threads.Add(messages_thread);
            threads.Add(photos_thread);
            threads.Add(posts_thread);
            threads.Add(progressbar_thread);
        }

        private void SilentUpdating()
        {
            m = new FacebookSN();
            while (true)
            {
                Thread.Sleep(60000);
                m.Person();
                m.Links();
                m.Messages();
                m.Photos();
                m.Posts();

                while (!ISocialNetwork.all_done)
                {
                    Dispatcher.Invoke(() => ISocialNetwork.all_done = ISocialNetwork.person_done && ISocialNetwork.links_done && ISocialNetwork.messages_done && ISocialNetwork.photos_done && ISocialNetwork.posts_done);
                    Thread.Sleep(100);
                }
                ISocialNetwork.RefreshStaticVariables();               
            }
        }

        private void ProgressController()
        {
            while (!ISocialNetwork.all_done)
            {
                RefreshProgress();
                Thread.Sleep(100);
            }

            System.Windows.MessageBox.Show("Done!");
            ISocialNetwork.RefreshStaticVariables();
            RefreshProgress();

            Thread silent_updating = new Thread(SilentUpdating);
            silent_updating.Start();
            threads.Add(silent_updating);

            Thread background_swaping = new Thread(WarpedStartSetBackground);
            background_swaping.SetApartmentState(ApartmentState.MTA);
            background_swaping.IsBackground = true;
            background_swaping.Start();
            threads.Add(background_swaping);
        }

        private void RefreshProgress()
        {
            Dispatcher.Invoke(() => person_progressbar.Maximum = ISocialNetwork.person_progressbar_max);
            Dispatcher.Invoke(() => links_progressbar.Maximum = ISocialNetwork.links_progressbar_max);
            Dispatcher.Invoke(() => messages_progressbar.Maximum = ISocialNetwork.messages_progressbar_max);
            Dispatcher.Invoke(() => photos_progressbar.Maximum = ISocialNetwork.photos_progressbar_max);
            Dispatcher.Invoke(() => posts_progressbar.Maximum = ISocialNetwork.posts_progressbar_max);

            Dispatcher.Invoke(() => person_progressbar.Value = ISocialNetwork.person_progressbar);
            Dispatcher.Invoke(() => links_progressbar.Value = ISocialNetwork.links_progressbar);
            Dispatcher.Invoke(() => messages_progressbar.Value = ISocialNetwork.messages_progressbar);
            Dispatcher.Invoke(() => photos_progressbar.Value = ISocialNetwork.photos_progressbar);
            Dispatcher.Invoke(() => posts_progressbar.Value = ISocialNetwork.posts_progressbar);

            Dispatcher.Invoke(() => PersonSaved.IsChecked = ISocialNetwork.person_done);
            Dispatcher.Invoke(() => LinksSaved.IsChecked = ISocialNetwork.links_done);
            Dispatcher.Invoke(() => MessagesSaved.IsChecked = ISocialNetwork.messages_done);
            Dispatcher.Invoke(() => PhotosSaved.IsChecked = ISocialNetwork.photos_done);
            Dispatcher.Invoke(() => PostsSaved.IsChecked = ISocialNetwork.posts_done);

            Dispatcher.Invoke(() => ISocialNetwork.all_done = ISocialNetwork.person_done && ISocialNetwork.links_done && ISocialNetwork.messages_done && ISocialNetwork.photos_done && ISocialNetwork.posts_done);
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
            AbortProgresses();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();
            
            //base.OnStateChanged(e);
        }

        private void AbortProgresses()
        {
            foreach(Thread t in threads)
            {
                if (t != null && t.IsAlive)
                    t.Abort();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AbortProgresses();
            Application.Current.Shutdown();
        }
    }
}
