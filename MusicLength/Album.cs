using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicLength
{
    public class Album
    {
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        public Artist Artist { get; set; }
        public BitmapImage Image { get; set; }
        public TimeSpan Duration
        {
            get
            {
                TimeSpan r = TimeSpan.Zero;
                foreach (Track track in Tracks)
                {
                    r += track.Duration;
                }
                return r;
            }
        }
        public int TrackCount
        {
            get
            {
                return Tracks.Count;
            }
        }

        public Album()
        {
            Tracks = new List<Track>();
        }

        public async Task Add(StorageFile f, MusicProperties p)
        {
            if (Image == null)
            {
                StorageItemThumbnail thumbnail = await f.GetThumbnailAsync(ThumbnailMode.MusicView);
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(thumbnail);
                Image = image;
            }
            Track t = new Track(f, p, Artist);
            Tracks.Add(t);
        }

        public AlbumListViewBinding ToBinding()
        {
            AlbumListViewBinding binding = new AlbumListViewBinding();
            binding.Image = Image;
            binding.Title = Name;
            binding.Duration = AlbumListViewBinding.FormatDuration(Duration);
            binding.Artist = Artist.Name;
            binding.album = this;
            return binding;
        }
    }
}
