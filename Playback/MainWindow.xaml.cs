using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Windows.Threading;

namespace Playback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mp3FileReader reader;
        private WaveOutEvent output;
        DispatcherTimer timer;
        bool dragging = false;
        private VolumeWaveProvider16 volumeProvider;


        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += OnTimerTick;

            LlenarComboDispositivos();
        }

        private void LlenarComboDispositivos()
        {
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capaciades =
                    WaveOut.GetCapabilities(i);
                cbDispositivos.Items.Add(capaciades.ProductName);
            }
            cbDispositivos.SelectedIndex = 0;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if(reader != null)
            {
                string tiempoActual = reader.CurrentTime.ToString();
                tiempoActual = tiempoActual.Substring(0, 8);
                lblPosition.Text = tiempoActual;
                if (!dragging)
                {
                    sldPosition.Value = reader.CurrentTime.TotalSeconds;
                }
            }
      }

        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog =
                new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                txtruta.Text = openFileDialog.FileName;
            }
        }

        private void btnplay_Click(object sender, RoutedEventArgs e)
        {
            if(output != null && output.PlaybackState == PlaybackState.Paused)
            {
                output.Play();
                btnplay.IsEnabled = false;
                btnPause.IsEnabled = true;
                btnstop.IsEnabled = true;
            }
            else
            {
                if (txtruta.Text != null && txtruta.Text != "")
                {
                    output = new WaveOutEvent();
                    output.PlaybackStopped += OnPlaybackStop;
                    reader = new Mp3FileReader(txtruta.Text);

                    //Configuraciones de WaveOut
                    output.DeviceNumber = cbDispositivos.SelectedIndex;
                    output.NumberOfBuffers = 2;
                    output.DesiredLatency = 100;

                   
                    output.Init(reader);
                    output.Play();

                    btnstop.IsEnabled = true;
                    btnPause.IsEnabled = true;
                    btnplay.IsEnabled = false;

                    lblDuration.Text = reader.TotalTime.ToString().Substring(0, 8);
                    lblPosition.Text = reader.CurrentTime.ToString().Substring(0, 8);

                    sldPosition.Maximum = reader.TotalTime.TotalSeconds;
                    sldPosition.Value = 0;
                    timer.Start();

                }
                else
                {
                    //Avisarle al usuario que elija un archivo
                }
            }
           
        }

        private void OnPlaybackStop(object sender, StoppedEventArgs e)
        {
            reader.Dispose();
            output.Dispose();
            timer.Stop();
        }

        private void btnstop_Click(object sender, RoutedEventArgs e)
        {
            if (reader != null && output != null && output.PlaybackState == PlaybackState.Playing ||
                output.PlaybackState == PlaybackState.Paused)
            {
                output.Stop();
                btnplay.IsEnabled = true;
                btnstop.IsEnabled = false;
            }
        }
       

        private void sldPosition_DragCompleted(object sender,RoutedEventArgs e)
        {
            if (reader != null)
            {
                reader.CurrentTime = TimeSpan.FromSeconds(sldPosition.Value);
                dragging = false;
            }
        }

        private void sldPosition_dragStarted(object sender, RoutedEventArgs e)
        {
            if (reader != null)
            {
                dragging = true;
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (output != null)
            {
                if (output.PlaybackState == PlaybackState.Playing)
                {
                    output.Pause();
                    btnPause.IsEnabled = false;
                    btnstop.IsEnabled = false;
                    btnplay.IsEnabled = true;
                }
            }
        }

        private void sldVolumen_DragCompleted(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
