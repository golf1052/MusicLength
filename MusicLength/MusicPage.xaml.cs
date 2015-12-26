using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicLength
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicPage : Page
    {
        TimeSpan timeSpan;

        ObservableCollection<AlbumListViewBinding> albums;
        SortedList<TimeSpan, Album> sortedAlbums;

        public MusicPage()
        {
            this.InitializeComponent();

            albums = new ObservableCollection<AlbumListViewBinding>();
            sortedAlbums = new SortedList<TimeSpan, Album>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timeSpan = (TimeSpan)e.Parameter;

            base.OnNavigatedTo(e);
        }

        private async void launchGrooveButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("mswindowsmusic://");
            await Launcher.LaunchUriAsync(uri);
        }

        private async void albumsListView_Loaded(object sender, RoutedEventArgs e)
        {
            albumsListView.ItemsSource = albums;

            foreach (Artist artist in AppConstants.Music)
            {
                foreach (Album album in artist.Albums)
                {
                    sortedAlbums.Add(album.Duration, album);
                }
            }
            foreach (KeyValuePair<TimeSpan, Album> album in sortedAlbums)
            {
                albums.Add(album.Value.ToBinding());
            }

            int i = 0;
            foreach (KeyValuePair<TimeSpan, Album> album in sortedAlbums)
            {
                if (album.Key > timeSpan)
                {
                    albumsListView.ScrollIntoView(albums[i]);
                    break;
                }
                i++;
            }

            await LoadGrooveData();
        }

        private async Task LoadGrooveData()
        {
            progressBar.IsIndeterminate = false;
            progressBar.Value = 0;
            progressBar.Maximum = albums.Count;
            progressTextBlock.Text = progressBar.Value + " of " + progressBar.Maximum +
                "\nretrieving Groove Music links";
            for (int i = 0; i < albums.Count; i++)
            {
                AlbumListViewBinding binding = albums[i];
                string url = "https://music.xboxlive.com/1/content/music/search?q=" +
                    binding.Title + " " + binding.Artist +
                    "&filters=albums";
                JObject o = await AppConstants.GetGrooveData(url);
                if (o != null)
                {
                    string albumName = (string)o["Albums"]["Items"][0]["Name"];
                    string artistName = (string)o["Albums"]["Items"][0]["Artists"][0]["Artist"]["Name"];
                    if (albumName == binding.Title &&
                        artistName == binding.Artist)
                    {
                        binding.Link = ((string)o["Albums"]["Items"][0]["Id"]).Split('.')[1];
                        binding.LinkSet = Visibility.Visible;
                        albums.RemoveAt(i);
                        albums.Insert(i, binding);
                    }
                }
                progressBar.Value++;
                progressTextBlock.Text = progressBar.Value + " of " + progressBar.Maximum +
                    "\nretrieving Groove Music links";
            }
            progressGrid.Visibility = Visibility.Collapsed;
        }

        private async void albumsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            AlbumListViewBinding binding = e.ClickedItem as AlbumListViewBinding;
            if (!string.IsNullOrEmpty(binding.Link))
            {
                await Launcher.LaunchUriAsync(new Uri("mswindowsmusic://details/?zestId=" + binding.Link + "&mediaType=album"));
            }
        }
    }
}
