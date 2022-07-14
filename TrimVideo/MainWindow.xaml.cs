using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using TrimVideo.Controls;
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
            }
            else
            {
                videoControl.Pause();
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
        }

        private void _OnTick()
        {
            if (!_isDragging)
            {
                _vm.VideoProgress = videoControl.Position;
            }
        }

        private void DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDragging = true;
            videoControl.IsMuted = true;
            videoControl.Pause();
        }

        private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            videoControl.Position = _vm.VideoProgress;
            _isDragging = false;
            videoControl.IsMuted = false;
            UpdatePlayState();
        }

        private void DragDelta(object sender, DragDeltaEventArgs e)
        {
            videoControl.Position = _vm.VideoProgress;
        }

        private void SliderLoaded(object sender, RoutedEventArgs e)
        {
            TripleThumbSlider slider = (TripleThumbSlider)sender;
            MouseButtonEventHandler handler = new ((sender, e) =>
                {
                    Track track = slider.Template.FindName("PART_Track", slider) as Track;

                    if (track == null || track.Thumb == null || track.Thumb.IsMouseOver) return;

                    track.Thumb.UpdateLayout();

                    track.Thumb.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                    {
                        RoutedEvent = MouseLeftButtonDownEvent,
                        Source = track.Thumb,
                    });

                    DragDelta(null, null);
                }
            );

            slider.AddHandler(PreviewMouseLeftButtonDownEvent, handler, true);

            UpdatePlayState();
        }
    }
}