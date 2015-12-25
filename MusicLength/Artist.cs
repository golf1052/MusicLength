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
    public class Artist
    {
        public string Name { get; set; }
        public List<Album> Albums { get; set; }
        public List<Track> OtherTracks { get; set; }
        public TimeSpan Duration
        {
            get
            {
                TimeSpan r = TimeSpan.Zero;
                foreach (Album a in Albums)
                {
                    r += a.Duration;
                }
                foreach (Track t in OtherTracks)
                {
                    r += t.Duration;
                }
                return r;
            }
        }

        public Artist()
        {
            Name = "";
            Albums = new List<Album>();
            OtherTracks = new List<Track>();
        }

        public async Task Add(StorageFile f, MusicProperties p)
        {
            foreach (Album a in Albums)
            {
                if (a.Name == p.Album)
                {
                    await a.Add(f, p);
                    return;
                }
            }
            if (!string.IsNullOrEmpty(p.Album))
            {
                Album album = new Album();
                album.Name = p.Album;
                album.Artist = this;
                await album.Add(f, p);
                Albums.Add(album);
            }
            else
            {
                //StorageItemThumbnail thumbnail = await f.GetThumbnailAsync(ThumbnailMode.MusicView);
                //BitmapImage image = new BitmapImage();
                //await image.SetSourceAsync(thumbnail);
                Track t = new Track(f, p, this);
                OtherTracks.Add(t);
            }
        }
    }
}
