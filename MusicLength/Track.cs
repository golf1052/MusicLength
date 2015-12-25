using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicLength
{
    public class Track
    {
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public uint TrackNumber { get; set; }
        public Album Album { get; set; }
        public Artist Artist { get; set; }

        public Track(StorageFile f, MusicProperties p, Artist a)
        {
            Name = f.DisplayName;
            Artist = a;
            TrackNumber = p.TrackNumber;
            Duration = p.Duration;
        }
    }
}
