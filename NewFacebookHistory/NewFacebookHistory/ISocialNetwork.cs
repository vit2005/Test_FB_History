using NewFacebookHistory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFacebookHistory
{
    abstract class ISocialNetwork
    {
        public static List<PhotoEnity> PhotosDatabase = new List<PhotoEnity>();
        public static bool person_done = false;
        public static bool links_done = false;
        public static bool messages_done = false;
        public static bool photos_done = false;
        public static bool posts_done = false;
        public static int person_progressbar_max = 100;
        public static int links_progressbar_max = 100;
        public static int messages_progressbar_max = 100;
        public static int photos_progressbar_max = 100;
        public static int posts_progressbar_max = 100;
        public static int person_progressbar = 0;
        public static int links_progressbar = 0;
        public static int messages_progressbar = 0;
        public static int photos_progressbar = 0;
        public static int posts_progressbar = 0;
        public static bool all_done;

        public static void RefreshStaticVariables()
        {
            person_done = false;
            links_done = false;
            messages_done = false;
            photos_done = false;
            posts_done = false;
            person_progressbar_max = 100;
            links_progressbar_max = 100;
            messages_progressbar_max = 100;
            photos_progressbar_max = 100;
            posts_progressbar_max = 100;
            person_progressbar = 0;
            links_progressbar = 0;
            messages_progressbar = 0;
            photos_progressbar = 0;
            posts_progressbar = 0;
        }

        public abstract void Person();
        public abstract void Links();
        public abstract void Messages();
        public abstract void Photos();
        public abstract void Posts();

    }
}
