using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NewFacebookHistory
{
    static class Settings
    {
        public static string login = string.Empty;
        public static string password = string.Empty;
        public static string folder = string.Empty;
        public static string accessToken = string.Empty;

        static Settings()
        {
            using (StreamReader sr = File.OpenText("Settings.ini"))
            {
                login = sr.ReadLine();
                password = sr.ReadLine();
                folder = sr.ReadLine();
            }
        }

        static public void Save()
        {
            using (StreamWriter sw = File.CreateText("Settings.ini"))
            {
                sw.WriteLine(login);
                sw.WriteLine(password);
                sw.WriteLine(folder);
            }
        }
    }
}
