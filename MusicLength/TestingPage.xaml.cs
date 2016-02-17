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
using System.Diagnostics;
using Windows.UI.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicLength
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestingPage : Page
    {
        ToolTip toolTip;

        public TestingPage()
        {
            this.InitializeComponent();
            toolTip = new ToolTip();
            ToolTipService.SetToolTip(sButton, toolTip);
        }

        private void sButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(sButton);
            Debug.WriteLine("pointer pressed at " + PointToString(point.Position));
        }

        private void sButton_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Debug.WriteLine("manipulation started, pointer moved " + e.Cumulative.Expansion);
            Debug.WriteLine("manipulation started, pointer moved " + PointToString(e.Cumulative.Translation));
            toolTip.IsOpen = true;
        }

        private void sButton_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Debug.WriteLine("manipulation delta, inertia");
            }
            Debug.WriteLine("manipulation delta, pointer moved " + e.Delta.Expansion);
            Debug.WriteLine("manipulation delta, pointer moved " + PointToString(e.Delta.Translation));
            Debug.WriteLine("manipulation delta, pointer moved in total " + e.Cumulative.Expansion);
            Debug.WriteLine("manipulation delta, pointer moved in total " + PointToString(e.Cumulative.Translation));
            int ratio = 1;
            if (e.Cumulative.Translation.Y != 0)
            {
                ratio = 1 + Math.Abs((int)e.Cumulative.Translation.Y / 50);
            }
            if (ratio > 10)
            {
                ratio = 10;
            }
            Debug.WriteLine("ratio " + ratio);
            Thickness thickness = sButton.Margin;
            thickness.Left += e.Delta.Translation.X / ratio;
            sButton.Margin = thickness;
            toolTip.Content = "ratio " + ratio;
            toolTip.HorizontalOffset += e.Delta.Translation.X / ratio;
        }

        private string PointToString(Point p)
        {
            return "x:" + p.X.ToString() + ", y:" + p.Y.ToString();
        }

        private void sButton_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Debug.WriteLine("manipulation completed, inertia");
            }
            Debug.WriteLine("manipulation completed, pointer moved " + e.Cumulative.Expansion);
            Debug.WriteLine("manipulation completed, pointer moved " + PointToString(e.Cumulative.Translation));
            Debug.WriteLine("manipulation completed, pointer is at " + PointToString(e.Position));
            toolTip.IsOpen = false;
        }

        private void MoveButton(Point p)
        {
            Thickness t = sButton.Margin;
            if (sButton.HorizontalAlignment == HorizontalAlignment.Right)
            {
                t.Right -= p.X;
            }
            else
            {
                t.Left += p.X;
            }
            sButton.Margin = t;
        }
    }
}
