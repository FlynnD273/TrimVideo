using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrimVideo.Controls
{
    /// <summary>
    /// Interaction logic for TripleThumbSlider.xaml
    /// </summary>
    public partial class TripleThumbSlider : UserControl
    {
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(0d, null, LowerValueCoerceValueCallback));
        public static readonly DependencyProperty MiddleValueProperty =
            DependencyProperty.Register("MiddleValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(1d, null, MiddleValueCoerceValueCallback));
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(1d, null, UpperValueCoerceValueCallback));
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(1d));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); MiddleValue = LowerValue; }
        }

        public double MiddleValue
        {
            get { return (double)GetValue(MiddleValueProperty); }
            set { SetValue(MiddleValueProperty, value); }
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); MiddleValue = UpperValue; }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public TripleThumbSlider()
        {
            InitializeComponent();
        }

        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Min(value, targetSlider.UpperValue);
        }

        private static object MiddleValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Min(Math.Max(value, targetSlider.LowerValue), targetSlider.MiddleValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Max(value, targetSlider.LowerValue);
        }

        private void SliderLoaded(object sender, RoutedEventArgs e)
        {
            MouseButtonEventHandler handler = new((sender, e) =>
            {
                Slider slider = sender as Slider;
                
                Track track = slider.Template.FindName("PART_Track", slider) as Track;

                if (!slider.IsMoveToPointEnabled || track == null || track.Thumb == null || track.Thumb.IsMouseOver) return;

                track.Thumb.UpdateLayout();

                track.Thumb.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = MouseLeftButtonDownEvent,
                    Source = track.Thumb,
                });
            }
            );

            (sender as Slider).AddHandler(PreviewMouseLeftButtonDownEvent, handler, true);
        }

        private enum DragMode
        {
            LowerValue,
            MiddleValue,
            UpperValue,
        }

        private DragMode _dragMode;

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas grid = (Canvas)sender;
            Point position = e.GetPosition(grid);

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / grid.Width * (Maximum - Minimum) + Minimum;

            if (xValue < LowerValue)
            {
                LowerValue = xValue;
                _dragMode = DragMode.LowerValue;
            }
            else if (xValue > UpperValue)
            {
                UpperValue = xValue;
                _dragMode = DragMode.UpperValue;
            }
            else
            {
                MiddleValue = xValue;
                _dragMode = DragMode.MiddleValue;
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Canvas parent = (Canvas)sender;
            Point position = e.GetPosition(parent);

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / parent.Width * (Maximum - Minimum) + Minimum;

            switch (_dragMode)
            {
                case DragMode.LowerValue:
                    LowerValue = xValue;
                    break;
                case DragMode.MiddleValue:
                    MiddleValue = xValue;
                    break;
                case DragMode.UpperValue:
                    UpperValue = xValue;
                    break;
            }
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
