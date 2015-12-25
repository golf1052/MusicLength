using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Search;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MusicLength
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageLibrary music = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            IObservableVector<StorageFolder> musicFolders = music.Folders;
            List<StorageFile> files = new List<StorageFile>();
            List<Artist> fullInfo = new List<Artist>();
            foreach (StorageFolder f in musicFolders)
            {
                files.AddRange(await GetFiles(f));
            }
            foreach (StorageFile f in files)
            {
                if (f.FileType != ".")
                {
                    MusicProperties musicProperties = await f.Properties.GetMusicPropertiesAsync();
                    fullInfo = Add(fullInfo, f, musicProperties);
                }
            }
            foreach (Artist artist in fullInfo)
            {
                Debug.WriteLine(artist.Name + " - " + artist.Duration);
                foreach (Album album in artist.Albums)
                {
                    Debug.WriteLine("\t" + album.Name + " - " + album.Duration);
                    foreach (Track t in album.Tracks)
                    {
                        Debug.WriteLine("\t\t" + t.Name + " - " + t.Duration);
                    }
                }
                foreach (Track t in artist.OtherTracks)
                {
                    Debug.WriteLine("\t" + t.Name + " - " + t.Duration);
                }
            }
            base.OnNavigatedTo(e);
        }

        async Task<List<StorageFile>> GetFiles(StorageFolder folder)
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
                    files.Add(i as StorageFile);
                }
            }
            return files;
        }

        List<Artist> Add(List<Artist> artists, StorageFile f, MusicProperties p)
        {
            foreach (Artist a in artists)
            {
                if (a.Name == p.AlbumArtist ||
                    a.Name == p.Artist)
                {
                    a.Add(f, p);
                    return artists;
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
            artist.Add(f, p);
            artists.Add(artist);
            return artists;
        }
    }
}
