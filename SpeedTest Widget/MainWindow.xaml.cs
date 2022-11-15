using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Net.NetworkInformation;

namespace SpeedTest_Widget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ping;
        string upload;
        string download;
        bool isClicked = false;
        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();      
        }
        #region starting Process and Filtering data
        public async Task SpeedTest()
        {
            //Starting speedtest.exe
            Process process = null;
           
                try
                {
                    process = new Process();

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = "--accept-license --accept-gdpr";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.FileName = "speedtest.exe";
                    process.Start();
                    //Resetting textblocks.text when process is started
                    Download.Text = "Download: ";
                    Ping.Text = "Ping: ";
                    Upload.Text = "Upload: ";

                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    //Filtering data
                    string[] wynik = output.Split(Environment.NewLine).ToArray();      
                    download = wynik[6].Split("(")[0].Trim();
                    upload = wynik[8].Split("(")[0].Trim();
                    ping = wynik[5].Split("(")[0].Trim();

                    ping = ping.Split(":")[1].Trim();
                    download = download.Split(":")[1].Trim();
                    upload = upload.Split(":")[1].Trim();
                }
                catch (Exception)
                {
                 
                Ping.Text = "Network Error";
                Download.Text = "Network Error";
                Upload.Text = "Network Error";

                throw;
                }
                finally
                {
                    if (process != null)
                    {
                        process.Close();
                        process.Dispose();
                    }
                }
            }


        #endregion
        #region mouseInteraction
        public async Task Manager()
        {
            await StartAnimation();
            await StartTimer();
            await SpeedTest(); 
        }
        //Start on click
        private void start_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isClicked)
            {
                Manager();
                isClicked = true;
            }
        }
        //Moving Window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        //Closing App
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }
        #endregion
        #region Timer and animation
        DispatcherTimer _timer = new DispatcherTimer();
        
        private void timer_Tick(object sender, EventArgs e) 
        {
            counter++;
            Timer.Text = counter.ToString();
            if (counter >= 100){
                //Deleting event handler and stopping timer
                _timer.Tick -= timer_Tick;
                _timer.Stop();
                
                counter = 0;
                Timer.Text = "START";
                cpb_uc.Visibility = Visibility.Collapsed;
                percent.Visibility = Visibility.Collapsed;
                //Showing data
                Download.Text = "Download: " + download;
                Ping.Text = "Ping: " + ping;
                Upload.Text = "Upload: " + upload;
                isClicked = false;
            }
        }
        private async Task StartTimer()
        {
            cpb_uc.Visibility = Visibility.Visible;
            percent.Visibility = Visibility.Visible;
      
            _timer.Interval = TimeSpan.FromMilliseconds(376);
            _timer.Tick += timer_Tick;
            _timer.Start();

        }
        private async Task StartAnimation()
        {
            ((Storyboard)cpb_uc.Resources["ProgressAnimation"]).Begin();
        }
        #endregion
    }
}
