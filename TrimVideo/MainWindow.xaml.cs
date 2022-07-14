using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using TrimVideo.Controls;
using TrimVideo.Converters;
using TrimVideo.ViewModels;

namespace TrimVideo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel _vm;

        private bool _isDragging = false;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (ViewModel)DataContext;
            _vm.PropertyChanged += _OnPropertyChanged;
        }

        private void _OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsPlaying):
                    UpdatePlayState();
                    break;
            }
        }

        private void UpdatePlayState()
        {
            if (_vm.IsPlaying)
            {
                videoControl.Play();
                _timer?.Start();
            }
            else
            {
                videoControl.Pause();
                _timer?.Stop();
            }
        }

        private void OnMediaLoaded(object sender, RoutedEventArgs e)
        {
            _vm.VideoLength = videoControl.NaturalDuration.TimeSpan;
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(200),
            };
            _timer.Tick += (_, _) => _OnTick();
            _timer.Start();

            videoControl.Position = TimeSpan.Zero;
        }

        private void _OnTick()
        {
            if (!_isDragging)
            {
                _vm.VideoProgress = videoControl.Position;
                if (videoControl.Position > _vm.VideoUpperBound)
                {
                    videoControl.Position = _vm.VideoUpperBound;
                    _vm.IsPlaying = false;
                }
            }
        }

        private void TripleThumbSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            videoControl.IsMuted = true;
            videoControl.Pause();
        }

        private void TripleThumbSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            videoControl.Position = _vm.VideoProgress;
            _isDragging = false;
            videoControl.IsMuted = false;
            UpdatePlayState();
        }

        private void TripleThumbSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            videoControl.Position = _vm.VideoProgress;
        }

        private void TripleThumbSlider_Loaded(object sender, RoutedEventArgs e)
        {
            videoControl.Play();
            UpdatePlayState();
        }

        private void TripleThumbSlider_Initialized(object sender, EventArgs e)
        {
            var slider = sender as TripleThumbSlider;
            slider.LowerValue = 0;
            slider.UpperValue = (double)new DoubleToTimeSpanConverter().Convert(_vm.VideoLength, null, null, null);
        }
    }
}