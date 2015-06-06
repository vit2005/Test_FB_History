using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFacebookSDK
{
    static class Settings
    {
        public static string login;
        public static string password;
        public static string folder;
        public static string accessToken;

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
