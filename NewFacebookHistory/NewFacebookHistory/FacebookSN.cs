using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using System.IO;
using System.Net;
using NewFacebookHistory.Entities;
using System.Threading;
using System.Windows.Threading;

namespace NewFacebookHistory
{
    class FacebookSN : ISocialNetwork
    {
        FacebookClient fb;
        public FacebookSN()
        {
            fb = new FacebookClient(Settings.accessToken);
        }

        override public void Person()
        {
            var person = (IDictionary<string, object>)fb.Get("me");
            SavePerson(person);
        }

        override public void Links()
        {
            var links = (IDictionary<string, object>)fb.Get("me/links");
            SaveLinks(links);
        }

        override public void Messages()
        {
            var messages = (IDictionary<string, object>)fb.Get("me/inbox");
            SaveMessages(messages);
        }

        override public void Photos()
        {
            var photos = (IDictionary<string, object>)fb.Get("me/photos/uploaded?fields=album");
            SavePhotos(photos);
        }

        override public void Posts()
        {
            var posts = (IDictionary<string, object>)fb.Get("me/feed");
            SavePosts(posts);
        }


        #region Messages
        private void SaveMessages(IDictionary<string, object> result)
        {
            //IDictionary<string, object> threads = (IDictionary<string, object>)result["data"];
            JsonArray threads = (JsonArray)result["data"];

            Dispatcher.CurrentDispatcher.Invoke(() => messages_progressbar_max = threads.Count);
            Dispatcher.CurrentDispatcher.Invoke(() => messages_progressbar = 0);
            foreach(object thread in threads)
            {
                IDictionary<string, object> data = (IDictionary<string, object>)thread;
                IDictionary<string, object> to_Dictionary = (IDictionary<string, object>)data["to"];

                JsonArray to_JsonArray = (JsonArray)to_Dictionary["data"];
                IDictionary<string, object> senderData = (IDictionary<string, object>)to_JsonArray[0];
                string name = (string)senderData["name"];
                SaveMessage(name, data);
                Dispatcher.CurrentDispatcher.Invoke(() => messages_progressbar++);
            }
            Dispatcher.CurrentDispatcher.Invoke(() => messages_done = true);
        }

        private void SaveMessage(string title, IDictionary<string, object> data)
        {
            System.IO.Directory.CreateDirectory(Settings.folder + "/Messages/");
            using (StreamWriter sw = File.CreateText(Settings.folder + "/Messages/" + title + ".txt"))
            {
                foreach (string o1 in data.Keys)
                {
                    sw.WriteLine("\""+o1+"\""+":");
                    if (o1 == "comments")
                    {
                        IDictionary<string, object> comments_data = (IDictionary<string, object>)data[o1];
                        //JsonArray comments_array = (JsonArray)comments_data["data"];
                        string next = "";
                        while (((JsonArray)comments_data["data"]).Count > 0)
                        {
                            JsonArray comments_array = (JsonArray)comments_data["data"];
                            foreach (object o2 in comments_array)
                            {
                                
                                IDictionary<string, object> comment_data = (IDictionary<string, object>)o2;
                                if (!comment_data.Keys.Contains("message"))
                                    continue;
                                IDictionary<string, object> me = new Dictionary<string, object>();
                                me.Add("name", "me");
                                IDictionary<string, object> from_data = (comment_data.Keys.Contains("from")) ? (IDictionary<string, object>)comment_data["from"] : me;
                                sw.WriteLine(comment_data["created_time"] + " " + from_data["name"] + ":");
                                sw.WriteLine(comment_data["message"] + "\n");
                            }
                            IDictionary<string, object> comments_paging = (IDictionary<string, object>)comments_data["paging"];
                            
                            comments_data = (IDictionary<string, object>)fb.Get(comments_paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", ""));
                            if (comments_paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", "") == next)
                                break;
                            next = comments_paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", "");
                            sw.WriteLine(" ========= Previous 25 messages ===========" + "\n");
                        }
                    }
                    else
                    {
                        sw.WriteLine(data[o1] + "\n");
                    }
                }
            }
        }

        #endregion


        #region Photos

        private void SavePhotos(IDictionary<string, object> result)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => photos_progressbar_max = 0);
            Preload(result);
            TrySavePhotosFromQuery(result);
            result = (IDictionary<string, object>)fb.Get("me/photos");
            TrySavePhotosFromQuery(result);
            
            PhotosDatabase.Sort(
                delegate(PhotoEnity p1, PhotoEnity p2)
                {
                    return p1.date.CompareTo(p2.date);
                });

            PhotosDatabase.Sort(
                delegate(PhotoEnity p1, PhotoEnity p2)
                {
                    return p1.likes.CompareTo(p2.likes);
                });
            Dispatcher.CurrentDispatcher.Invoke(() => photos_done = true);
        }

        private void TrySavePhotosFromQuery(IDictionary<string, object> result)
        {
            JsonArray pictures_array = (JsonArray)result["data"];
            bool stillHavePhotos = true;
            while (stillHavePhotos)
            {
                foreach (object picture in pictures_array)
                {
                    SavePhoto((IDictionary<string, object>)picture);
                    Dispatcher.CurrentDispatcher.Invoke(() => photos_progressbar++);
                }

                IDictionary<string, object> paging = (IDictionary<string, object>)result["paging"];
                if (paging.ContainsKey("next"))
                {
                    result = (IDictionary<string, object>)fb.Get(paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", ""));
                    pictures_array = (JsonArray)result["data"];
                }
                else
                    stillHavePhotos = false;
            }
        }

        private void Preload(IDictionary<string, object> result)
        {
            TryPreloadFromQuery(result);
            result = (IDictionary<string, object>)fb.Get("me/photos");
            TryPreloadFromQuery(result);
        }

        private void TryPreloadFromQuery(IDictionary<string, object> result)
        {            
            IDictionary<string, object> paging = (IDictionary<string, object>)result["paging"];

            if (!paging.ContainsKey("next"))
            {
                Dispatcher.CurrentDispatcher.Invoke(() => photos_progressbar_max += ((JsonArray)result["data"]).Count);
                return;
            }

            while (((IDictionary<string, object>)result["paging"]).ContainsKey("next"))
            {
                Dispatcher.CurrentDispatcher.Invoke(() => photos_progressbar_max += ((JsonArray)result["data"]).Count);
                result = (IDictionary<string, object>)fb.Get(((IDictionary<string, object>)result["paging"])["next"].ToString().Replace("https://graph.facebook.com/v2.3/", ""));
            }
        }

        private void SavePhoto(IDictionary<string, object> result)
        {
            IDictionary<string, object> album;
            if (result.ContainsKey("album"))
                album = (IDictionary<string, object>)result["album"];
            else
            {
                album = new Dictionary<string, object>();
                album.Add(new KeyValuePair<string, object>("name", "Photos with me"));
            }
            
            string folder = Settings.folder + "/Photos/" + album["name"].ToString();
            string id = result["id"].ToString();

            System.IO.Directory.CreateDirectory(folder);
            if (album.ContainsKey("id"))
                SaveAlbumDescription(album, string.Format("{0}/{1}.txt", Settings.folder + "/Photos/", album["name"].ToString()));
            IDictionary<string, object> image_data = (IDictionary<string, object>)fb.Get(id);
            JsonArray images_array = (JsonArray)image_data["images"];
            IDictionary<string, object> image = (IDictionary<string, object>)images_array[0];
            string source = (string)image["source"];
            if (!File.Exists(string.Format("{0}/{1}.jpg", folder, id)))
                DownloadPhoto(folder, source, id);

            SavePhotoDescription(folder, image_data, id);
        }

        private void SaveAlbumDescription(IDictionary<string, object> album, string file)
        {
            if (File.Exists(file))
                return;

            IDictionary<string, object> album_data = (IDictionary<string, object>)fb.Get(album["id"].ToString());

            using (StreamWriter sw = File.CreateText(file))
            {
                sw.WriteLine("id:");
                sw.WriteLine(album_data["id"]);

                sw.WriteLine("created_time:");
                sw.WriteLine(album_data["created_time"]);

                if (album_data.Keys.Contains("likes"))
                {
                    sw.WriteLine("likes:");

                    IDictionary<string, object> likes_data = (IDictionary<string, object>)album_data["likes"];
                    JsonArray likes_array = (JsonArray)likes_data["data"];
                    foreach(object like in likes_array)
                    {
                        IDictionary<string, object> like_data = (IDictionary<string, object>)like;
                        sw.WriteLine(string.Format("{0} (id:{1})", like_data["name"], like_data["id"]));
                    }
                }

                if (album_data.Keys.Contains("comments"))
                {
                    sw.WriteLine("comments:");

                    IDictionary<string, object> comments_data = (IDictionary<string, object>)album_data["comments"];
                    JsonArray comments_array = (JsonArray)comments_data["data"];
                    foreach (object comment in comments_array)
                    {
                        IDictionary<string, object> comment_data = (IDictionary<string, object>)comment;
                        IDictionary<string, object> sender_data = (IDictionary<string, object>)comment_data["from"];
                        sw.WriteLine(string.Format("{0} {1} (id:{2}) likes:{3}", comment_data["created_time"].ToString().Replace("T", " ").Replace("+0000", ""), sender_data["name"], sender_data["id"], comment_data["like_count"]));
                        sw.WriteLine(comment_data["message"]);
                        sw.WriteLine();
                    }
                }
            }
        }

        private void DownloadPhoto(string folder, string url, string id)
        {
            using (WebClient Client = new WebClient())
            {
                Client.DownloadFile(url, string.Format("{0}/{1}.jpg", folder, id));
            }
        }

        private void SavePhotoDescription(string folder, IDictionary<string, object> image_data, string id)
        {
            
            using (StreamWriter sw = File.CreateText(string.Format("{0}/{1}.txt",folder, id)))
            {
                PhotoEnity pe = new PhotoEnity();
                pe.uri = string.Format("{0}/{1}.JPG", folder, id);

                sw.WriteLine("id:");
                sw.WriteLine(image_data["id"]);

                sw.WriteLine("created_time:");
                sw.WriteLine(image_data["created_time"]);
                pe.date = DateTime.Parse(image_data["created_time"].ToString());

                if (image_data.Keys.Contains("updated_time"))
                {
                    sw.WriteLine("updated_time:");
                    sw.WriteLine(image_data["updated_time"]);
                }

                if (image_data.Keys.Contains("likes"))
                {
                    sw.WriteLine("likes:");
                    IDictionary<string, object> likes_data = (IDictionary<string, object>)image_data["likes"];
                    JsonArray likes_array = (JsonArray)likes_data["data"];
                    foreach (object like in likes_array)
                    {
                        IDictionary<string, object> like_data = (IDictionary<string, object>)like;
                        sw.WriteLine(string.Format("{0} (id:{1})", like_data["name"], like_data["id"]));
                    }
                    pe.likes = likes_array.Count;
                }

                if (image_data.Keys.Contains("comments"))
                {
                    JsonArray comments_array = (JsonArray)((IDictionary<string, object>)image_data["comments"])["data"];
                    foreach (object comment in comments_array)
                    {
                        IDictionary<string, object> comment_data = (IDictionary<string, object>)comment;
                        IDictionary<string, object> from = (IDictionary<string, object>)comment_data["from"];

                        sw.WriteLine(string.Format("{0} {1}(id:{2}) likes:{3}", comment_data["created_time"].ToString().Replace("T", " ").Replace("+0000", ""), from["name"].ToString(), from["id"], comment_data["like_count"]));
                        sw.WriteLine(comment_data["message"] + "\n");
                    }
                }

                PhotosDatabase.Add(pe);
            }
        }

        #endregion


        #region Posts

        private void SavePosts(IDictionary<string, object> result)
        {
            IDictionary<string, object> paging = (IDictionary<string, object>)result["paging"];
            string next = "";
            System.IO.Directory.CreateDirectory(Settings.folder + "/Posts");
            while (((JsonArray)result["data"]).Count > 0)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => posts_progressbar_max = ((JsonArray)result["data"]).Count);
                Dispatcher.CurrentDispatcher.Invoke(() => posts_progressbar = 0);

                foreach(object o in (JsonArray)result["data"])
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => posts_progressbar++);
                    #region save post
                    IDictionary<string, object> post = (IDictionary<string, object>)o;
                    if (post["type"].ToString() != "status")
                        continue;

                    string url = Settings.folder + "/Posts/" + post["created_time"].ToString().Replace("T", " ").Replace("+0000", "").Replace(":", "_") + ".txt";
                    using (StreamWriter sw = File.CreateText(url))
                    {
                        if (post.Keys.Contains("story"))
                        {
                            sw.WriteLine("Story:");
                            sw.WriteLine(post["story"].ToString());
                        }

                        if (post.Keys.Contains("name"))
                        {
                            sw.WriteLine("Name:");
                            sw.WriteLine(post["name"].ToString());
                        }

                        if (post.Keys.Contains("description"))
                        {
                            sw.WriteLine("Description:");
                            sw.WriteLine(post["description"].ToString());
                        }

                        if (post.Keys.Contains("message"))
                        {
                            sw.WriteLine("Message:");
                            sw.WriteLine(post["message"].ToString());
                        }

                        if (post.Keys.Contains("link"))
                        {
                            sw.WriteLine("Link:");
                            sw.WriteLine(post["link"].ToString());
                        }

                        if (post.Keys.Contains("likes"))
                        {
                            sw.WriteLine("likes:");
                            IDictionary<string, object> likes_data = (IDictionary<string, object>)post["likes"];
                            JsonArray likes_array = (JsonArray)likes_data["data"];
                            foreach (object like in likes_array)
                            {
                                IDictionary<string, object> like_data = (IDictionary<string, object>)like;
                                sw.WriteLine(string.Format("{0} (id:{1})", like_data["name"], like_data["id"]));
                            }
                        }

                        if (post.Keys.Contains("comments"))
                        {
                            sw.WriteLine("Comments:");
                            IDictionary<string, object> comments = (IDictionary<string, object>)post["comments"];
                            foreach(object comment in (JsonArray)comments["data"])
                            {
                                IDictionary<string, object> comment_data = (IDictionary<string, object>)comment;
                                IDictionary<string, object> from = (IDictionary<string, object>)comment_data["from"];
                                sw.WriteLine(string.Format("{0} {1}(id:{2}) likes:{3}", comment_data["created_time"].ToString().Replace("T", " ").Replace("+0000", ""), from["name"].ToString(), from["id"], comment_data["like_count"]));
                                sw.WriteLine(comment_data["message"] + "\n");
                            }
                        }
                    }
                    #endregion
                }

                result = (IDictionary<string, object>)fb.Get(paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", ""));

                if (paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", "") == next)
                    break;

                next = paging["next"].ToString().Replace("https://graph.facebook.com/v2.3/", "");
            }
            Dispatcher.CurrentDispatcher.Invoke(() => posts_done = true);
        }

        #endregion


        #region Person

        private void SavePerson(IDictionary<string, object> result)
        {

            List<string> param = new List<string>() { "id", "about", "bio", "birthday", "email", "first_name", "middle_name", "last_name", "name", "gender", "link", "political", "relationship_status", "religion", "quotes", "website", "updated_time" };
            using (StreamWriter sw = File.CreateText(Settings.folder + "/Person.txt"))
            {
                Dispatcher.CurrentDispatcher.Invoke(() => person_progressbar_max = param.Count);
                Dispatcher.CurrentDispatcher.Invoke(() => person_progressbar = 0);

                foreach(string s in param)
                {
                    if (result.Keys.Contains(s))
                    {
                        sw.WriteLine(s+":");
                        sw.WriteLine(result[s].ToString());
                        sw.WriteLine();
                    }
                    Dispatcher.CurrentDispatcher.Invoke(() => person_progressbar++);
                }
            }
            Dispatcher.CurrentDispatcher.Invoke(() => person_done = true);
        }

        #endregion


        #region Links

        private void SaveLinks(IDictionary<string, object> result)
        {
            using (StreamWriter sw = File.CreateText(Settings.folder + "/Links.txt"))
            {
                Dispatcher.CurrentDispatcher.Invoke(() => links_progressbar_max = ((JsonArray)result["data"]).Count);
                Dispatcher.CurrentDispatcher.Invoke(() => links_progressbar = 0);

                foreach (object o in (JsonArray)result["data"])
                {
                    IDictionary<string, object> link_data = (IDictionary<string, object>)o;
                    sw.WriteLine(link_data["created_time"].ToString().Replace("T", " ").Replace("+0000", ""));
                    sw.WriteLine(link_data["link"]);

                    if (link_data.Keys.Contains("likes"))
                    {
                        sw.WriteLine("likes:");
                        IDictionary<string, object> likes_data = (IDictionary<string, object>)link_data["likes"];
                        JsonArray likes_array = (JsonArray)likes_data["data"];
                        foreach (object like in likes_array)
                        {
                            IDictionary<string, object> like_data = (IDictionary<string, object>)like;
                            sw.WriteLine(string.Format("{0} (id:{1})", like_data["name"], like_data["id"]));
                        }
                    }
                    Dispatcher.CurrentDispatcher.Invoke(() => links_progressbar++);
                    sw.WriteLine();
                }
            }
            Dispatcher.CurrentDispatcher.Invoke(() => links_done = true);
        }

        #endregion
    }
}
