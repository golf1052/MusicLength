using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Task loadingTask = AppConstants.LoadMusic();
            while (!loadingTask.IsCompleted)
            {
                if (AppConstants.NumberRemaining == -1)
                {
                    progressBar.IsIndeterminate = true;
                }
                else
                {
                    progressBar.IsIndeterminate = false;
                    progressBar.Value = AppConstants.NumberCompleted;
                    progressBar.Maximum = AppConstants.NumberRemaining;
                }
                progressTextBlock.Text = AppConstants.ProgressText;
                await Task.Delay(1);
            }
            Frame.Navigate(typeof(TimePickerPage));
            base.OnNavigatedTo(e);
        }
    }
}
