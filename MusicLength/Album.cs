using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicLength
{
    public class Album
    {
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        public Artist Artist { get; set; }
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

        public void Add(StorageFile f, MusicProperties p)
        {
            Track t = new Track(f, p, Artist);
            Tracks.Add(t);
        }
    }
}
