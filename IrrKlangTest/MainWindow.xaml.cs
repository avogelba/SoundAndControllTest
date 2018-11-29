using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using IrrKlang;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using TagLib;

namespace IrrKlangTest
{

    //Just a test for https://www.ambiera.com/irrklang/index.html
    //https://www.ambiera.com/irrklang/docunet/index.html


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        public static int JUMP_TIME = 1000 * 15; //15 seconds
        protected ISoundEngine engine = null;
        protected ISound sound = null;
        protected string file = String.Empty;
        protected TimeSpan timeSpan;
        protected string lenghth = String.Empty;
        protected string position = String.Empty;
        protected bool? _loopSound = false;
        protected Brush blackBrush = new SolidColorBrush(Color.FromArgb(255,0, 0, 0));
        protected Brush redBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        public bool? loopSound
        {
            get { return _loopSound; }
            set { _loopSound = value; }
        }

        DispatcherTimer dispatcherTimer;


        public MainWindow()
        {
            InitializeComponent();
            engine = new ISoundEngine();
            DataContext = this;
            
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
//            volCtrl.Volume = 100;


        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (file != null && file != String.Empty)
            {
                if (sound != null)
                    sound.Stop();
                //bool loopIt = chkLoop.IsChecked.HasValue ? chkLoop.IsChecked.Value : false;
                //sound = engine.Play2D(file,loopIt);
                sound = engine.Play2D(file, _loopSound ?? false);
                setVolume();
                //sound.setSoundStopEventReceiver(soundStopEvent);
                timeSpan = TimeSpan.FromMilliseconds(sound.PlayLength);
                lenghth = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                //txtLenghth.Text = lenghth;
                stsPrg.IsIndeterminate = true;
                stsStatus.Content = "Playing...";
                play1.Fill = redBrush;
                stopped1.Fill = blackBrush;
            }
        }

        //private ISoundStopEventReceiver soundStopEvent()
        //{
        //    updateStop();
        //}
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All playable files (*.mp3;*.ogg;*.wav;*.mod;*.it;*.xm;*.it;*.s3d)|*.mp3;*.ogg;*.wav;*.mod;*.it;*.xm;*.it;*.s3d";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == true)
            {
                file = openFileDialog.FileName;
                txtFile.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                stsStatus.Content = "Ready...";
                //Playing with TagLib Sharp: https://github.com/mono/taglib-sharp
                //Supports Audio-Tags, according web-page, for: aa, aax, aac, aiff, ape, dsf, flac, m4a, m4b, m4p, mp3, mpc, mpp, ogg, oga, wav, wma, wv, webm
                try
                {
                    var tfile = TagLib.File.Create(file);
                    if (tfile != null)
                    {
                        string myAlbum = tfile.Tag.Album;
                        if(myAlbum != null && myAlbum != String.Empty)
                        {
                            stsAlb.Content = myAlbum;
                        }
                        else
                        {
                            stsAlb.Content = "-";
                        }
                    }
                }
                catch
                {
                    stsAlb.Content = "?";
                }


            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (sound != null)
            {
                sound.Stop();
                sound.Paused = false;
                updateStop();
            }

        }

        private void updateStop()
        {
            stsPrg.IsIndeterminate = false;
            stsStatus.Content = "Stopped...";
            pause1.Fill = pause2.Fill = sound.Paused ? redBrush : blackBrush;
            play1.Fill = blackBrush;
            stopped1.Fill = redBrush;
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (sound != null)
            {
                sound.Paused = !sound.Paused;
                stsPrg.IsIndeterminate = !stsPrg.IsIndeterminate;
                stsStatus.Content = sound.Paused ? "Paused..." : "Playing...";
                pause1.Fill = pause2.Fill = sound.Paused ? redBrush : blackBrush;
                
            }

        }

        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            if (sound!=null && file != String.Empty)
            {
                if (sound.PlayPosition != 0 && sound.PlayLength !=0) //skip 
                {
                    //txtCurPos.Text = sound.PlayPosition.ToString()+" - " +sound.PlayLength.ToString();
                    //prgPos.Value = (double)sound.PlayPosition/ (double)sound.PlayLength * 100d;//(sound.PlayLength / 100) * (100 / sound.PlayPosition) *100;
                    prgPos.Maximum = (double)sound.PlayLength;
                    prgPos.Value = (double)sound.PlayPosition;
                    
                    //txtProgress.Text = prgPos.Value.ToString();
                    timeSpan = TimeSpan.FromMilliseconds(sound.PlayPosition);
                    position = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                    //txtPos.Text = position;
                    prgPos.ToolTip = position;
                }
                if (sound.Finished)
                {
                    updateStop();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            volCtrl.Volume = 100;
            stsPrg.IsIndeterminate = false;
            stsStatus.Content = "Waiting...";
            chkLoop.IsChecked = true;
        }

        private void volCtrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (volCtrl2.Volume != volCtrl.Volume)
                volCtrl2.Volume = volCtrl.Volume;
            setVolume();


        }

        private void setVolume()
        {
            if (sound != null)
            {
                sound.Volume = (float)(volCtrl.Volume / 100f);


            }
        }

        private void volCtrl2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (volCtrl2.Volume != volCtrl.Volume)
                volCtrl.Volume = volCtrl2.Volume;
            if (sound != null)
            {
                sound.Volume = (float)(volCtrl2.Volume / 100f);
             
            }
            
        }

        private void chkLoop_Unchecked(object sender, RoutedEventArgs e)
        {
            chkLoop.IsChecked = !chkLoop.IsChecked;
        }

        private void chkLoop_Checked(object sender, RoutedEventArgs e)
        {
            chkLoop.IsChecked = !chkLoop.IsChecked;
        }

        private void chkLoop_Click(object sender, RoutedEventArgs e)
        {
            if(sound!=null)
                sound.Looped = _loopSound ?? false;
            
        }

        private void btnFBW_Click(object sender, RoutedEventArgs e)
        {
            if (sound != null)
            {
                int test = (int)sound.PlayPosition - JUMP_TIME;
                sound.PlayPosition = (test >= 0) ? (uint)test : 0;
            }
        }

        private void btnFF_Click(object sender, RoutedEventArgs e)
        {
            if (sound != null)
            {
                int test = (int)sound.PlayPosition + JUMP_TIME;
                sound.PlayPosition = (test <= sound.PlayLength) ? (uint)test : sound.PlayLength;
            }
        }
    }
}
