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
    public partial class TripleThumbSlider : UserControl
    {
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(0d));
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(0d, LowerValuePropertyChangedCallback, LowerValueCoerceValueCallback));
        public static readonly DependencyProperty MiddleValueProperty =
            DependencyProperty.Register("MiddleValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(0d, MiddleValuePropertyChangedCallback, MiddleValueCoerceValueCallback));
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(TripleThumbSlider), new UIPropertyMetadata(1d, UpperValuePropertyChangedCallback, UpperValueCoerceValueCallback));
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
            set { SetValue(LowerValueProperty, value); }
        }

        public double MiddleValue
        {
            get { return (double)GetValue(MiddleValueProperty); }
            set { SetValue(MiddleValueProperty, value); }
        }

        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        private double _xOffset;

        public TripleThumbSlider()
        {
            InitializeComponent();
            SizeChanged += (_, _) => _UpdateAllPositions();
        }

        private void _UpdateAllPositions()
        {
            var middleValue = MiddleValue;
            LowerValuePropertyChangedCallback(this, new());
            MiddleValuePropertyChangedCallback(this, new());
            UpperValuePropertyChangedCallback(this, new());
            MiddleValue = middleValue;
        }

        private static void LowerValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = (TripleThumbSlider)d;
            var lowerThumb = t.lowerThumb;
            lowerThumb.SetValue(Canvas.LeftProperty, (t.LowerValue - t.Minimum) / (t.Maximum - t.Minimum) * t.canvas.ActualWidth);

            t.MiddleValue = t.LowerValue;
        }

        private static void MiddleValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = (TripleThumbSlider)d;
            var middleThumb = t.middleThumb;
            middleThumb.SetValue(Canvas.LeftProperty, (t.MiddleValue - t.Minimum) / (t.Maximum - t.Minimum) * t.canvas.ActualWidth);
        }

        private static void UpperValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = (TripleThumbSlider)d;
            var upperThumb = t.upperThumb;
            upperThumb.SetValue(Canvas.LeftProperty, (t.UpperValue - t.Minimum) / (t.Maximum - t.Minimum) * t.canvas.ActualWidth);

            t.MiddleValue = t.UpperValue;
        }

        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Min(Math.Max(Math.Min(value, targetSlider.UpperValue), targetSlider.Minimum), targetSlider.Maximum);
        }

        private static object MiddleValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Min(Math.Max(value, targetSlider.LowerValue), targetSlider.UpperValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            return Math.Min(Math.Max(Math.Max(value, targetSlider.LowerValue), targetSlider.Minimum), targetSlider.Maximum);
        }

        private enum DragMode
        {
            LowerValue,
            MiddleValue,
            UpperValue,
        }

        private DragMode _dragMode;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas parent = (Canvas)sender;
            Point position = e.GetPosition(parent);

            FrameworkElement over = (FrameworkElement)Mouse.DirectlyOver;
            if (over == parent)
            {
                _xOffset = 0;
            }
            else
            {
                _xOffset = (Canvas.GetLeft(over) - position.X) / parent.ActualWidth * (Maximum - Minimum) + Minimum;
            }

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / parent.ActualWidth * (Maximum - Minimum) + Minimum + _xOffset;

            bool onThumb = false;
            if (over == lowerThumb)
            {
                LowerValue = xValue;
                onThumb = true;
                _dragMode = DragMode.LowerValue;
            }
            else if (over == upperThumb)
            {
                UpperValue = xValue;
                onThumb = true;
                _dragMode = DragMode.UpperValue;
            }
            else if (over == middleThumb)
            {
                MiddleValue = xValue;
                onThumb = true;
                _dragMode = DragMode.MiddleValue;
            }

            parent.CaptureMouse();

            if (onThumb) return;

            if (xValue <= LowerValue)
            {
                LowerValue = xValue;
                _dragMode = DragMode.LowerValue;
            }
            else if (xValue >= UpperValue)
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

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Canvas parent = (Canvas)sender;

            if (e.LeftButton != MouseButtonState.Pressed || !parent.IsMouseCaptured) return;

            Point position = e.GetPosition(parent);

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / parent.ActualWidth * (Maximum - Minimum) + Minimum + _xOffset;

            switch (_dragMode)
            {
                case DragMode.LowerValue:
                    LowerValue = xValue;
                    break;
                case DragMode.MiddleValue:
                    MiddleValue = Math.Min(Math.Max(xValue, LowerValue), UpperValue);
                    break;
                case DragMode.UpperValue:
                    UpperValue = xValue;
                    break;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas parent = (Canvas)sender;
            if (!parent.IsMouseCaptured) return;

            if (_dragMode == DragMode.UpperValue) MiddleValue = LowerValue;

            parent.ReleaseMouseCapture();
        }
    }
}
