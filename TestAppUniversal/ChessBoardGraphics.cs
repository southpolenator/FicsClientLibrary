﻿using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUniversal
{
    class ChessBoardGraphics
    {
        public static Brush DrawField(bool white)
        {
            LinearGradientBrush brush = new LinearGradientBrush();

            brush.StartPoint = white ? new Point(0, 0) : new Point(0, 1);
            brush.EndPoint = white ? new Point(1, 1) : new Point(1, 0);
            GradientStop stop1 = new GradientStop();
            stop1.Color = white ? Color.FromArgb(255, 239, 231, 186) : Color.FromArgb(255, 254, 0, 0);
            stop1.Offset = 0.5;
            brush.GradientStops.Add(stop1);
            GradientStop stop2 = new GradientStop();
            stop2.Color = white ? Color.FromArgb(255, 191, 167, 127) : Color.FromArgb(255, 169, 0, 0);
            stop2.Offset = 1;
            brush.GradientStops.Add(stop2);
            return brush;
        }

        public static void DrawFieldMark(Canvas canvas)
        {
            Rectangle rectangle = new Rectangle();

            canvas.Width = 45;
            canvas.Height = 45;
            rectangle.Width = canvas.Width;
            rectangle.Height = canvas.Height;
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            rectangle.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 255, 0));
            canvas.Children.Add(rectangle);
        }
    }
}
