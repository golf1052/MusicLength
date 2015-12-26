using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicLength
{
    public class AlbumListViewBinding
    {
        public BitmapImage Image { get; set; }
        public string Duration { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Link { get; set; }
        public Visibility LinkSet { get; set; }
        public Album album;

        public AlbumListViewBinding()
        {
            LinkSet = Visibility.Collapsed;
        }

        public static string FormatDuration(TimeSpan timeSpan)
        {
            string r = string.Empty;
            if (timeSpan.Hours != 0)
            {
                r += timeSpan.Hours + " hours ";
            }
            r += timeSpan.Minutes + " minutes ";
            r += timeSpan.Seconds + " seconds";
            return r;
        }
    }
}
