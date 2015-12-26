using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicLength
{
    public static class AppConstants
    {
        public static List<Artist> Music { get; set; }
        public static string ProgressText { get; private set; }
        public static int NumberCompleted { get; private set; }
        public static int NumberRemaining { get; private set; }
        private static string accessToken;
        private static DateTime expiresIn;

        static AppConstants()
        {
            Music = new List<Artist>();
            expiresIn = DateTime.Now;
        }

        public static async Task LoadMusic()
        {
            NumberCompleted = 0;
            NumberRemaining = -1;
            ProgressText = "found " + NumberCompleted + " tracks";
            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            List<StorageFile> musicFiles = new List<StorageFile>();
            foreach (StorageFolder f in musicLibrary.Folders)
            {
                musicFiles.AddRange(await GetFiles(f));
            }
            NumberRemaining = NumberCompleted;
            NumberCompleted = 0;
            ProgressText = "loading " + NumberCompleted + " of " + NumberRemaining + " tracks";
            foreach (StorageFile f in musicFiles)
            {
                MusicProperties musicProperties = await f.Properties.GetMusicPropertiesAsync();
                await Add(f, musicProperties);
                NumberCompleted++;
                ProgressText = "loading " + NumberCompleted + " of " + NumberRemaining + " tracks";
            }
        }

        private static async Task<List<StorageFile>> GetFiles(StorageFolder folder)
        {
            IReadOnlyList<IStorageItem> items = await folder.GetItemsAsync();
            List<StorageFile> files = new List<StorageFile>();
            foreach (IStorageItem i in items)
            {
                if (i.IsOfType(StorageItemTypes.Folder))
                {
                    files.AddRange(await GetFiles((StorageFolder)i));
                }
                else
                {
                    StorageFile f = i as StorageFile;
                    if (f.ContentType.StartsWith("audio"))
                    {
                        files.Add(f);
                        NumberCompleted++;
                        ProgressText = "found " + NumberCompleted + " tracks";
                    }
                }
            }
            return files;
        }

        private static async Task Add(StorageFile f, MusicProperties p)
        {
            foreach (Artist a in Music)
            {
                if (a.Name == p.AlbumArtist ||
                    a.Name == p.Artist)
                {
                    await a.Add(f, p);
                    return;
                }
            }
            Artist artist = new Artist();
            if (!string.IsNullOrEmpty(p.AlbumArtist))
            {
                artist.Name = p.AlbumArtist;
            }
            else if (!string.IsNullOrEmpty(p.Artist))
            {
                artist.Name = p.Artist;
            }
            await artist.Add(f, p);
            Music.Add(artist);
        }

        public static async Task<string> GetAccessToken()
        {
            if (string.IsNullOrEmpty(accessToken) ||
                DateTime.Now > expiresIn)
            {
                await RefreshAccessToken();
            }
            return accessToken;
        }

        private static async Task RefreshAccessToken()
        {
            expiresIn = DateTime.Now;
            HttpClient httpClient = new HttpClient();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("client_id", Secrets.ClientId);
            data.Add("client_secret", Secrets.ClientSecret);
            data.Add("scope", "http://music.xboxlive.com");
            data.Add("grant_type", "client_credentials");
            HttpResponseMessage response = await httpClient.PostAsync(new Uri("https://datamarket.accesscontrol.windows.net/v2/OAuth2-13"), new FormUrlEncodedContent(data));
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            accessToken = (string)responseObject["access_token"];
            expiresIn += TimeSpan.FromSeconds((double)responseObject["expires_in"]);
        }

        public static async Task<JObject> GetGrooveData(string url)
        {
            url += "&accessToken=Bearer " + WebUtility.UrlEncode(await GetAccessToken());
            JObject r = null;
            HttpResponseMessage response = null;
            do
            {
                HttpClient httpClient = new HttpClient();
                response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    r = JObject.Parse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    if ((int)response.StatusCode == 429)
                    {
                        System.Diagnostics.Debug.WriteLine("rate limit");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }
                }
            }
            while ((int)response.StatusCode == 429);
            return r;
        }
    }
}
