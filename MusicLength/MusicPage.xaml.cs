using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public MusicPage()
        {
            this.InitializeComponent();

            albums = new ObservableCollection<AlbumListViewBinding>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timeSpan = (TimeSpan)e.Parameter;

            foreach (Artist artist in AppConstants.Music)
            {
                foreach (Album album in artist.Albums)
                {
                    albums.Add(album.ToBinding());
                }
            }

            base.OnNavigatedTo(e);
        }

        private async void launchGrooveButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("mswindowsmusic://");
            await Launcher.LaunchUriAsync(uri);
        }

        private void albumsListView_Loaded(object sender, RoutedEventArgs e)
        {
            albumsListView.ItemsSource = albums;
        }
    }
}
