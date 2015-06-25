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
using System.Windows.Shapes;
using AForge.Video.FFMPEG;
using AviFile;
using System.Drawing;
using System.IO;
using NAudio.Wave;
using System.Windows.Forms;
using NewFacebookHistory.Entities;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace NewFacebookHistory
{
    /// <summary>
    /// Логика взаимодействия для MakeVideoWindow.xaml
    /// </summary>
    public partial class MakeVideoWindow : Window
    {
        private string filename;
        private static int progress = 0;
        private static int progress_max = 100;
        private DateTime from;
        private DateTime due;
        private int seconds;
        private TimeSpan time_length;
        private string audio_dilename;
        private bool canPlay = false;

        public MakeVideoWindow()
        {
            InitializeComponent();
            if (FacebookSN.PhotosDatabase.Count == 0)
            {
                System.Windows.MessageBox.Show("Please, download data");
                Start_button.IsEnabled = false;
            }
            else
                initializeFields();
            
        }

        private void initializeFields()
        {
            DateTime date = FacebookSN.PhotosDatabase.Min(x => x.date);
            DateFrom.SelectedDate = date;
            date = FacebookSN.PhotosDatabase.Max(x => x.date);
            DateDue.SelectedDate = date;
        }

        private void ProgressLoop()
        {
            while (!canPlay)
            {
                Dispatcher.Invoke(() => ProgressBar.Value = progress);
                Dispatcher.Invoke(() => ProgressBar.Maximum = progress_max);
                Thread.Sleep(100);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "mp3 files (*.mp3)|*.mp3" ;
            openFileDialog1.FilterIndex = 2 ;
            openFileDialog1.RestoreDirectory = true ;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                audio_textbox.Text = openFileDialog1.FileName;
            }
        }

        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            from = DateFrom.SelectedDate.Value;
            due = DateDue.SelectedDate.Value;
            seconds = Convert.ToInt32(minutes_textbox.Text) * 60 + Convert.ToInt32(seconds_textbox.Text);
            audio_dilename = audio_textbox.Text;
            time_length = new TimeSpan(0, Convert.ToInt32(minutes_textbox.Text), Convert.ToInt32(seconds_textbox.Text));

            Thread t = new Thread(InThreadMethod);
            t.Start();
            Thread p = new Thread(ProgressLoop);
            p.Start();
        }

        private void InThreadMethod()
        {
            WriteVideo();
            if (audio_dilename == string.Empty)
                audio_dilename = "Default.mp3";

            if (ConvertAudio())
            {
                TrimWavFile(System.IO.Path.GetTempPath() + "temp.wav", System.IO.Path.GetTempPath() + "temp2.wav", new TimeSpan(0), time_length);
                File.Delete(System.IO.Path.GetTempPath() + "temp.wav");
                File.Move(System.IO.Path.GetTempPath() + "temp2.wav", System.IO.Path.GetTempPath() + "temp.wav");
            }
                
            WriteAudio();
            System.Windows.MessageBox.Show("Done!");

            canPlay = true;
            progress = 0;
            File.Delete(System.IO.Path.GetTempPath() + "temp.wav");
            Process.Start(filename);
        }

        private void WriteVideo()
        {
            int width = 250, height = 250;
            VideoFileWriter writer = new VideoFileWriter();
            filename = string.Format("{0}/{1}.{2}.{3}-{4}.{5}.{6}.avi", Settings.folder, from.Year, from.Month, from.Day, due.Year, due.Month, due.Day);
            
            Bitmap ResizedPhoto;
            List<string> list = new List<string>();
            foreach (PhotoEnity photo in FacebookSN.PhotosDatabase)
                if (photo.date >= from && photo.date <= due.AddDays(1))
                    list.Add(photo.uri);

            int fps = seconds * 25 / list.Count();
            System.Drawing.Image imageBackground = System.Drawing.Image.FromFile("background.jpg");
            
            Dispatcher.Invoke(() => progress_max = list.Count());
            writer.Open(filename, width, height, 25, VideoCodec.MPEG4);
            foreach (string s in list)
            {
                //using (Graphics grfx = Graphics.FromImage(image))
                //{
                //    grfx.DrawImage(newImage, x, y);
                //}
                
                           

                System.Drawing.Image OriginalPhoto = System.Drawing.Image.FromFile(s);
                int w = OriginalPhoto.Width;
                int h = OriginalPhoto.Height;
                double scalew, scaleh;
                if (w > h)
                {
                    scalew = 1;
                    scaleh = (double)h / (double)w;
                } else{
                    scaleh = 1;
                    scalew = (double)w / (double)h;
                }
                ResizedPhoto = new Bitmap(OriginalPhoto, (int)(250 * scalew), (int)(250 * scaleh));

                System.Drawing.Image img = new Bitmap(imageBackground.Width, imageBackground.Height);
                using (Graphics gr = Graphics.FromImage(img))
                {
                    gr.DrawImage(imageBackground, new System.Drawing.Point(0, 0));
                    if (scalew == 1)
                    {
                        gr.DrawImage((System.Drawing.Image)ResizedPhoto, new System.Drawing.Point(0, 125 - ResizedPhoto.Height/2));
                    } else {
                        gr.DrawImage((System.Drawing.Image)ResizedPhoto, new System.Drawing.Point(125 - ResizedPhoto.Width / 2, 0));
                    }
                    
                }

                ResizedPhoto = new Bitmap(img);
                for (int i = 0; i < fps; i++)
                    writer.WriteVideoFrame(ResizedPhoto);
                
                Dispatcher.Invoke(() => progress++);
                ResizedPhoto.Dispose();
            }
            writer.Close();
        }

        private bool ConvertAudio()
        {
            bool res = false;
            using (Mp3FileReader reader = new Mp3FileReader(audio_dilename))
            {
                res = reader.TotalTime > time_length;
                using (WaveStream convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    string file = System.IO.Path.GetTempPath() + "temp.wav";
                    WaveFileWriter.CreateWaveFile(file, convertedStream);
                }
            }
            return res;
        }

        private void WriteAudio()
        {
            AviManager aviManager = new AviManager(filename, true);
            aviManager.AddAudioStream(System.IO.Path.GetTempPath() + "temp.wav", 0);
            aviManager.Close();
        }

        public static void TrimWavFile(string inPath, string outPath, TimeSpan cutFromStart, TimeSpan cutFromEnd)
        {
            using (WaveFileReader reader = new WaveFileReader(inPath))
            {
                using (WaveFileWriter writer = new WaveFileWriter(outPath, reader.WaveFormat))
                {
                    int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)cutFromStart.TotalMilliseconds * bytesPerMillisecond;
                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    int endBytes = (int)cutFromEnd.TotalMilliseconds * bytesPerMillisecond;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    //int endPos = (int)reader.Length - endBytes;

                    //TrimWavFile(reader, writer, startPos, endPos);
                    TrimWavFile(reader, writer, startPos, endBytes);
                }
            }
        }

        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.WriteData(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            advanture_settings.Visibility = ((sender as System.Windows.Controls.CheckBox).IsChecked.Value) ? Visibility.Visible : Visibility.Collapsed;
            this.Height = ((sender as System.Windows.Controls.CheckBox).IsChecked.Value) ? 237 : 135;
        }
    }
}
