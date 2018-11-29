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

namespace WpfVolumeSlider
{
    //Initial Idea from https://stackoverflow.com/questions/8547248/volume-slider-customcontrol
    // and https://stackoverflow.com/questions/8989854/volume-slider-like-vlc

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class VolumeSlider : UserControl, INotifyPropertyChanged
    {
        private double _volume;
        //private bool mouseCaptured = false;

        public double Volume
        {
            get { return _volume; }
            set
            {


                //_volume = 10 * value;
                _volume = value;
                OnPropertyChanged("Volume");
            }
        }
        public VolumeSlider()
        {
            InitializeComponent();
            DataContext = this;
        }
        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            //var test = WPFVolumeSlider.Height;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
