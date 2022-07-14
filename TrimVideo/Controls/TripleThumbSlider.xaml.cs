﻿using System;
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

        public TripleThumbSlider()
        {
            InitializeComponent();
        }

        private void _UpdateMiddleThumb()
        {
            middleThumb.SetValue(Canvas.LeftProperty, (MiddleValue - Minimum) / (Maximum - Minimum) * canvas.ActualWidth - middleThumb.ActualWidth / 2);
        }

        private void _UpdateLowerThumb()
        {
            lowerThumb.SetValue(Canvas.LeftProperty, (LowerValue - Minimum) / (Maximum - Minimum) * canvas.ActualWidth - lowerThumb.ActualWidth / 2);
        }

        private void _UpdateUpperThumb()
        {
            upperThumb.SetValue(Canvas.LeftProperty, (UpperValue - Minimum) / (Maximum - Minimum) * canvas.ActualWidth - upperThumb.ActualWidth / 2);
        }

        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            targetSlider._UpdateLowerThumb();
            double realValue = Math.Min(Math.Max(Math.Min(value, targetSlider.UpperValue), targetSlider.Minimum), targetSlider.Maximum);
            targetSlider.MiddleValue = realValue;
            targetSlider._UpdateMiddleThumb();

            return realValue;
        }

        private static object MiddleValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            targetSlider._UpdateMiddleThumb();
            double realValue = Math.Min(Math.Max(value, targetSlider.LowerValue), targetSlider.UpperValue);

            return realValue;
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            TripleThumbSlider targetSlider = (TripleThumbSlider)target;
            double value = (double)valueObject;

            targetSlider._UpdateUpperThumb();
            double realValue = Math.Min(Math.Max(Math.Max(value, targetSlider.LowerValue), targetSlider.Minimum), targetSlider.Maximum);
            targetSlider.MiddleValue = realValue;
            targetSlider._UpdateMiddleThumb();

            return realValue;
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
            Canvas parent = (Canvas)sender;
            Point position = e.GetPosition(parent);

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / parent.ActualWidth * (Maximum - Minimum) + Minimum;

            var over = Mouse.DirectlyOver;

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

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Canvas parent = (Canvas)sender;

            if (!parent.IsMouseCaptured) 
                return;

            Point position = e.GetPosition(parent);

            // Remap range from 0 - grid width to Minimum - Maximum
            double xValue = position.X / parent.ActualWidth * (Maximum - Minimum) + Minimum;

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

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            Canvas parent = (Canvas)sender;

            parent.ReleaseMouseCapture();
        }
    }
}
