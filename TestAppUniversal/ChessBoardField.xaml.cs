using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TestAppUniversal
{
    public sealed partial class ChessBoardField : UserControl
    {
        private static readonly Brush WhiteBrush = new SolidColorBrush(Colors.White);
        private static readonly Brush BlackBrush = new SolidColorBrush(Colors.Black);

        private bool white;
        private ChessPieceWithColor chessPiece;

        public ChessBoardField()
        {
            InitializeComponent();
            this.White = true;
        }

        public bool White
        {
            get { return white; }
            set { UpdateField(ref white, value); }
        }

        public ChessPieceWithColor ChessPiece
        {
            get { return chessPiece; }
            set { UpdateField(ref chessPiece, value); }
        }

        private void UpdateField<T>(ref T variable, T value)
        {
            if (!value.Equals(variable))
            {
                variable = value;
                UpdateLook();
            }
            else
            {
                variable = value;
            }
        }

        private bool IsChessPieceWhite
        {
            get { return this.ChessPiece.Color == ChessPieceColor.White; }
        }

        private void UpdateLook()
        {
            LinearGradientBrush brush = new LinearGradientBrush();

            brush.StartPoint = this.White ? new Point(0, 0) : new Point(0, 1);
            brush.EndPoint = this.White ? new Point(1, 1) : new Point(1, 0);
            GradientStop stop1 = new GradientStop();
            stop1.Color = this.White ? Color.FromArgb(255, 239, 231, 186) : Color.FromArgb(255, 254, 0, 0);
            stop1.Offset = 0.5;
            brush.GradientStops.Add(stop1);
            GradientStop stop2 = new GradientStop();
            stop2.Color = this.White ? Color.FromArgb(255, 191, 167, 127) : Color.FromArgb(255, 169, 0, 0);
            stop2.Offset = 1;
            brush.GradientStops.Add(stop2);
            this.LayoutRoot.Background = brush;

            this.Canvas.Children.Clear();
            if (ChessPiece != null)
            {
                IEnumerable<Shape> shapes = ConvertChessPieceToPaths(this.ChessPiece);

                this.Canvas.Width = 45;
                this.Canvas.Height = 45;
                foreach (Shape shape in shapes)
                {
                    this.Canvas.Children.Add(shape);
                }
            }
        }

        private IEnumerable<Shape> ConvertChessPieceToPaths(ChessPieceWithColor chessPiece)
        {
            if (chessPiece.Color == ChessPieceColor.White)
            {
                switch (chessPiece.Type)
                {
                    case ChessPieceType.Bishop:
                        return CreateWhiteBishopShapes();
                    case ChessPieceType.King:
                        return CreateWhiteKingShapes();
                    case ChessPieceType.Knight:
                        return CreateWhiteKnightShapes();
                    case ChessPieceType.Pawn:
                        return CreateWhitePawnShapes();
                    case ChessPieceType.Queen:
                        return CreateWhiteQueenShapes();
                    case ChessPieceType.Rook:
                        return CreateWhiteRookShapes();
                    default:
                        throw new Exception("Unknown ChessPieceType " + chessPiece.Type);
                }
            }
            else
            {
                switch (chessPiece.Type)
                {
                    case ChessPieceType.Bishop:
                        return CreateBlackBishopShapes();
                    case ChessPieceType.King:
                        return CreateBlackKingShapes();
                    case ChessPieceType.Knight:
                        return CreateBlackKnightShapes();
                    case ChessPieceType.Pawn:
                        return CreateBlackPawnShapes();
                    case ChessPieceType.Queen:
                        return CreateBlackQueenShapes();
                    case ChessPieceType.Rook:
                        return CreateBlackRookShapes();
                    default:
                        throw new Exception("Unknown ChessPieceType " + chessPiece.Type);
                }
            }
        }

        private static List<Path> CreateBlackBishopShapes()
        {
            List<Path> paths = CreatePaths(BlackBishop, BlackBrush, BlackBrush);

            paths[3].Fill = null;
            paths[3].Stroke = WhiteBrush;
            return paths;
        }

        private static List<Path> CreateBlackKingShapes()
        {
            List<Path> paths = CreatePaths(BlackKing, BlackBrush, null);

            paths[1].Fill = BlackBrush;
            paths[2].Fill = BlackBrush;
            paths[4].Stroke = WhiteBrush;
            paths[5].Stroke = WhiteBrush;
            return paths;
        }

        private static List<Path> CreateBlackKnightShapes()
        {
            List<Path> paths = CreatePaths(BlackKnight, WhiteBrush, WhiteBrush);

            paths[0].Fill = BlackBrush;
            paths[0].Stroke = BlackBrush;
            paths[1].Fill = BlackBrush;
            paths[1].Stroke = BlackBrush;
            paths[4].Stroke = null;
            return paths;
        }

        private static List<Path> CreateBlackPawnShapes()
        {
            return CreatePaths(BlackPawn, BlackBrush, BlackBrush);
        }

        private static IEnumerable<Shape> CreateBlackQueenShapes()
        {
            List<Path> paths = CreatePaths(BlackQueen, WhiteBrush, null);

            paths[0].Fill = BlackBrush;
            paths[0].Stroke = BlackBrush;
            paths[1].Fill = BlackBrush;
            paths[1].Stroke = BlackBrush;
            paths[2].Stroke = BlackBrush;
            foreach (Path path in paths)
            {
                yield return path;
            }

            yield return CreateCircle(6, 12, 2.75, null, BlackBrush);
            yield return CreateCircle(14, 9, 2.75, null, BlackBrush);
            yield return CreateCircle(22.5, 8, 2.75, null, BlackBrush);
            yield return CreateCircle(31, 9, 2.75, null, BlackBrush);
            yield return CreateCircle(39, 12, 2.75, null, BlackBrush);
        }

        private static List<Path> CreateBlackRookShapes()
        {
            List<Path> paths = CreatePaths(BlackRook, BlackBrush, BlackBrush);

            paths[6].Fill = null;
            paths[6].Stroke = WhiteBrush;
            paths[6].StrokeThickness = 1;
            paths[7].Fill = null;
            paths[7].Stroke = WhiteBrush;
            paths[7].StrokeThickness = 1;
            paths[8].Fill = null;
            paths[8].Stroke = WhiteBrush;
            paths[8].StrokeThickness = 1;
            paths[9].Fill = null;
            paths[9].Stroke = WhiteBrush;
            paths[9].StrokeThickness = 1;
            paths[10].Fill = null;
            paths[10].Stroke = WhiteBrush;
            paths[10].StrokeThickness = 1;
            return paths;
        }

        private static List<Path> CreateWhiteBishopShapes()
        {
            return CreatePaths(WhiteBishop, BlackBrush, WhiteBrush);
        }

        private static List<Path> CreateWhiteKingShapes()
        {
            return CreatePaths(WhiteKing, BlackBrush, WhiteBrush);
        }

        private static List<Path> CreateWhiteKnightShapes()
        {
            return CreatePaths(WhiteKnight, BlackBrush, WhiteBrush);
        }

        private static List<Path> CreateWhitePawnShapes()
        {
            return CreatePaths(WhitePawn, BlackBrush, WhiteBrush);
        }

        private static List<Path> CreateWhiteQueenShapes()
        {
            List<Path> paths = CreatePaths(WhiteQueen, BlackBrush, WhiteBrush);

            paths[0].RenderTransform = new TranslateTransform() { X = -1, Y = -1 };
            paths[1].RenderTransform = new TranslateTransform() { X = 15.5, Y = -5.5 };
            paths[2].RenderTransform = new TranslateTransform() { X = 32, Y = -1 };
            paths[3].RenderTransform = new TranslateTransform() { X = 7, Y = -4.5 };
            paths[4].RenderTransform = new TranslateTransform() { X = 24, Y = -4 };
            paths[7].Fill = null;
            paths[8].Fill = null;
            return paths;
        }

        private static List<Path> CreateWhiteRookShapes()
        {
            return CreatePaths(WhiteRook, BlackBrush, WhiteBrush);
        }

        private static Ellipse CreateCircle(double x, double y, double radius, Brush strokeBrush, Brush fillBrush)
        {
            return new Ellipse()
            {
                Height = radius * 2,
                Width = radius * 2,
                Stroke = strokeBrush,
                Fill = fillBrush,
                StrokeThickness = 1.5,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeMiterLimit = 4,
                Margin = new Thickness(x - radius, y - radius, 0, 0)
            };
        }

        private static List<Path> CreatePaths(string[] pathData, Brush strokeBrush, Brush fillBrush)
        {
            List<Path> paths = new List<Path>();

            foreach (string data in pathData)
            {
                Path path = CreatePath(data);

                path.Fill = fillBrush;
                path.Stroke = strokeBrush;
                path.StrokeThickness = 1.5;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeLineJoin = PenLineJoin.Round;
                path.StrokeMiterLimit = 4;
                paths.Add(path);
            }

            return paths;
        }

        private static Path CreatePath(string pathData)
        {
            return ((Path)XamlReader.Load("<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Data='" + pathData + "'/>"));
        }

        #region Path strings
        public static readonly string[] BlackBishop = new string[]
            {
                "M 9,36 C 12.39,35.03 19.11,36.43 22.5,34 C 25.89,36.43 32.61,35.03 36,36 C 36,36 37.65,36.54 39,38 C 38.32,38.97 37.35,38.99 36,38.5 C 32.61,37.53 25.89,38.96 22.5,37.5 C 19.11,38.96 12.39,37.53 9,38.5 C 7.646,38.99 6.677,38.97 6,38 C 7.354,36.06 9,36 9,36 z",
                "M 15,32 C 17.5,34.5 27.5,34.5 30,32 C 30.5,30.5 30,30 30,30 C 30,27.5 27.5,26 27.5,26 C 33,24.5 33.5,14.5 22.5,10.5 C 11.5,14.5 12,24.5 17.5,26 C 17.5,26 15,27.5 15,30 C 15,30 14.5,30.5 15,32 z",
                "M 25 8 A 2.5 2.5 0 1 1  20,8 A 2.5 2.5 0 1 1  25 8 z",
                "M 17.5,26 L 27.5,26 M 15,30 L 30,30 M 22.5,15.5 L 22.5,20.5 M 20,18 L 25,18",
            };

        public static readonly string[] BlackKing = new string[]
            {
                "M 22.5,11.63 L 22.5,6",
                "M 22.5,25 C 22.5,25 27,17.5 25.5,14.5 C 25.5,14.5 24.5,12 22.5,12 C 20.5,12 19.5,14.5 19.5,14.5 C 18,17.5 22.5,25 22.5,25",
                "M 11.5,37 C 17,40.5 27,40.5 32.5,37 L 32.5,30 C 32.5,30 41.5,25.5 38.5,19.5 C 34.5,13 25,16 22.5,23.5 L 22.5,27 L 22.5,23.5 C 19,16 9.5,13 6.5,19.5 C 3.5,25.5 11.5,29.5 11.5,29.5 L 11.5,37 z",
                "M 20,8 L 25,8",
                "M 32,29.5 C 32,29.5 40.5,25.5 38.03,19.85 C 34.15,14 25,18 22.5,24.5 L 22.51,26.6 L 22.5,24.5 C 20,18 9.906,14 6.997,19.85 C 4.5,25.5 11.85,28.85 11.85,28.85",
                "M 11.5,30 C 17,27 27,27 32.5,30 M 11.5,33.5 C 17,30.5 27,30.5 32.5,33.5 M 11.5,37 C 17,34 27,34 32.5,37",
            };

        public static readonly string[] BlackKnight = new string[]
            {
                "M 22,10 C 32.5,11 38.5,18 38,39 L 15,39 C 15,30 25,32.5 23,18",
                "M 24,18 C 24.38,20.91 18.45,25.37 16,27 C 13,29 13.18,31.34 11,31 C 9.958,30.06 12.41,27.96 11,28 C 10,28 11.19,29.23 10,30 C 9,30 5.997,31 6,26 C 6,24 12,14 12,14 C 12,14 13.89,12.1 14,10.5 C 13.27,9.506 13.5,8.5 13.5,7.5 C 14.5,6.5 16.5,10 16.5,10 L 18.5,10 C 18.5,10 19.28,8.008 21,7 C 22,7 22,10 22,10",
                "M 9.5 25.5 A 0.5 0.5 0 1 1 8.5,25.5 A 0.5 0.5 0 1 1 9.5 25.5 z",
                "M 15 15.5 A 0.5 1.5 0 1 1  14,15.5 A 0.5 1.5 0 1 1  15 15.5 z",
                "M 24.55,10.4 L 24.1,11.85 L 24.6,12 C 27.75,13 30.25,14.49 32.5,18.75 C 34.75,23.01 35.75,29.06 35.25,39 L 35.2,39.5 L 37.45,39.5 L 37.5,39 C 38,28.94 36.62,22.15 34.25,17.66 C 31.88,13.17 28.46,11.02 25.06,10.5 L 24.55,10.4 z",
            };

        public static readonly string[] BlackPawn = new string[]
            {
                "M 22,9 C 19.79,9 18,10.79 18,13 C 18,13.89 18.29,14.71 18.78,15.38 C 16.83,16.5 15.5,18.59 15.5,21 C 15.5,23.03 16.44,24.84 17.91,26.03 C 14.91,27.09 10.5,31.58 10.5,39.5 L 33.5,39.5 C 33.5,31.58 29.09,27.09 26.09,26.03 C 27.56,24.84 28.5,23.03 28.5,21 C 28.5,18.59 27.17,16.5 25.22,15.38 C 25.71,14.71 26,13.89 26,13 C 26,10.79 24.21,9 22,9 z",
            };

        public static readonly string[] BlackQueen = new string[]
            {
                "M 9,26 C 17.5,24.5 30,24.5 36,26 L 38.5,13.5 L 31,25 L 30.7,10.9 L 25.5,24.5 L 22.5,10 L 19.5,24.5 L 14.3,10.9 L 14,25 L 6.5,13.5 L 9,26 z",
                "M 9,26 C 9,28 10.5,28 11.5,30 C 12.5,31.5 12.5,31 12,33.5 C 10.5,34.5 10.5,36 10.5,36 C 9,37.5 11,38.5 11,38.5 C 17.5,39.5 27.5,39.5 34,38.5 C 34,38.5 35.5,37.5 34,36 C 34,36 34.5,34.5 33,33.5 C 32.5,31 32.5,31.5 33.5,30 C 34.5,28 36,28 36,26 C 27.5,24.5 17.5,24.5 9,26 z",
                "M 11,38.5 A 35,35 1 0 0 34,38.5",
                "M 11,29 A 35,35 1 0 1 34,29",
                "M 12.5,31.5 L 32.5,31.5",
                "M 11.5,34.5 A 35,35 1 0 0 33.5,34.5",
                "M 10.5,37.5 A 35,35 1 0 0 34.5,37.5",
            };

        public static readonly string[] BlackRook = new string[]
            {
                "M 9,39 L 36,39 L 36,36 L 9,36 L 9,39 z",
                "M 12.5,32 L 14,29.5 L 31,29.5 L 32.5,32 L 12.5,32 z",
                "M 12,36 L 12,32 L 33,32 L 33,36 L 12,36 z",
                "M 14,29.5 L 14,16.5 L 31,16.5 L 31,29.5 L 14,29.5 z",
                "M 14,16.5 L 11,14 L 34,14 L 31,16.5 L 14,16.5 z",
                "M 11,14 L 11,9 L 15,9 L 15,11 L 20,11 L 20,9 L 25,9 L 25,11 L 30,11 L 30,9 L 34,9 L 34,14 L 11,14 z",
                "M 12,35.5 L 33,35.5 L 33,35.5",
                "M 13,31.5 L 32,31.5",
                "M 14,29.5 L 31,29.5",
                "M 14,16.5 L 31,16.5",
                "M 11,14 L 34,14",
            };

        public static readonly string[] WhiteBishop = new string[]
            {
                "M 9,36 C 12.39,35.03 19.11,36.43 22.5,34 C 25.89,36.43 32.61,35.03 36,36 C 36,36 37.65,36.54 39,38 C 38.32,38.97 37.35,38.99 36,38.5 C 32.61,37.53 25.89,38.96 22.5,37.5 C 19.11,38.96 12.39,37.53 9,38.5 C 7.646,38.99 6.677,38.97 6,38 C 7.354,36.06 9,36 9,36 z",
                "M 15,32 C 17.5,34.5 27.5,34.5 30,32 C 30.5,30.5 30,30 30,30 C 30,27.5 27.5,26 27.5,26 C 33,24.5 33.5,14.5 22.5,10.5 C 11.5,14.5 12,24.5 17.5,26 C 17.5,26 15,27.5 15,30 C 15,30 14.5,30.5 15,32 z",
                "M 25 8 A 2.5 2.5 0 1 1  20,8 A 2.5 2.5 0 1 1  25 8 z",
                "M 17.5,26 L 27.5,26 M 15,30 L 30,30 M 22.5,15.5 L 22.5,20.5 M 20,18 L 25,18",
            };

        public static readonly string[] WhiteKing = new string[]
            {
                "M 22.5,11.63 L 22.5,6",
                "M 20,8 L 25,8",
                "M 22.5,25 C 22.5,25 27,17.5 25.5,14.5 C 25.5,14.5 24.5,12 22.5,12 C 20.5,12 19.5,14.5 19.5,14.5 C 18,17.5 22.5,25 22.5,25",
                "M 11.5,37 C 17,40.5 27,40.5 32.5,37 L 32.5,30 C 32.5,30 41.5,25.5 38.5,19.5 C 34.5,13 25,16 22.5,23.5 L 22.5,27 L 22.5,23.5 C 19,16 9.5,13 6.5,19.5 C 3.5,25.5 11.5,29.5 11.5,29.5 L 11.5,37 z",
                "M 11.5,30 C 17,27 27,27 32.5,30",
                "M 11.5,33.5 C 17,30.5 27,30.5 32.5,33.5",
                "M 11.5,37 C 17,34 27,34 32.5,37",
            };

        public static readonly string[] WhiteKnight = new string[]
            {
                "M 22,10 C 32.5,11 38.5,18 38,39 L 15,39 C 15,30 25,32.5 23,18",
                "M 24,18 C 24.38,20.91 18.45,25.37 16,27 C 13,29 13.18,31.34 11,31 C 9.958,30.06 12.41,27.96 11,28 C 10,28 11.19,29.23 10,30 C 9,30 5.997,31 6,26 C 6,24 12,14 12,14 C 12,14 13.89,12.1 14,10.5 C 13.27,9.506 13.5,8.5 13.5,7.5 C 14.5,6.5 16.5,10 16.5,10 L 18.5,10 C 18.5,10 19.28,8.008 21,7 C 22,7 22,10 22,10",
                "M 9.5 25.5 A 0.5 0.5 0 1 1 8.5,25.5 A 0.5 0.5 0 1 1 9.5 25.5 z",
                "M 15 15.5 A 0.5 1.5 0 1 1  14,15.5 A 0.5 1.5 0 1 1  15 15.5 z",
            };

        public static readonly string[] WhitePawn = new string[]
            {
                "M 22,9 C 19.79,9 18,10.79 18,13 C 18,13.89 18.29,14.71 18.78,15.38 C 16.83,16.5 15.5,18.59 15.5,21 C 15.5,23.03 16.44,24.84 17.91,26.03 C 14.91,27.09 10.5,31.58 10.5,39.5 L 33.5,39.5 C 33.5,31.58 29.09,27.09 26.09,26.03 C 27.56,24.84 28.5,23.03 28.5,21 C 28.5,18.59 27.17,16.5 25.22,15.38 C 25.71,14.71 26,13.89 26,13 C 26,10.79 24.21,9 22,9 z",
            };

        public static readonly string[] WhiteQueen = new string[]
            {
                "M 9 13 A 2 2 0 1 1  5,13 A 2 2 0 1 1  9 13 z",
                "M 9 13 A 2 2 0 1 1  5,13 A 2 2 0 1 1  9 13 z",
                "M 9 13 A 2 2 0 1 1  5,13 A 2 2 0 1 1  9 13 z",
                "M 9 13 A 2 2 0 1 1  5,13 A 2 2 0 1 1  9 13 z",
                "M 9 13 A 2 2 0 1 1  5,13 A 2 2 0 1 1  9 13 z",
                "M 9,26 C 17.5,24.5 30,24.5 36,26 L 38,14 L 31,25 L 31,11 L 25.5,24.5 L 22.5,9.5 L 19.5,24.5 L 14,10.5 L 14,25 L 7,14 L 9,26 z",
                "M 9,26 C 9,28 10.5,28 11.5,30 C 12.5,31.5 12.5,31 12,33.5 C 10.5,34.5 10.5,36 10.5,36 C 9,37.5 11,38.5 11,38.5 C 17.5,39.5 27.5,39.5 34,38.5 C 34,38.5 35.5,37.5 34,36 C 34,36 34.5,34.5 33,33.5 C 32.5,31 32.5,31.5 33.5,30 C 34.5,28 36,28 36,26 C 27.5,24.5 17.5,24.5 9,26 z",
                "M 11.5,30 C 15,29 30,29 33.5,30",
                "M 12,33.5 C 18,32.5 27,32.5 33,33.5",
            };

        public static readonly string[] WhiteRook = new string[]
            {
                "M 9,39 L 36,39 L 36,36 L 9,36 L 9,39 z",
                "M 12,36 L 12,32 L 33,32 L 33,36 L 12,36 z",
                "M 11,14 L 11,9 L 15,9 L 15,11 L 20,11 L 20,9 L 25,9 L 25,11 L 30,11 L 30,9 L 34,9 L 34,14",
                "M 34,14 L 31,17 L 14,17 L 11,14",
                "M 31,17 L 31,29.5 L 14,29.5 L 14,17",
                "M 31,29.5 L 32.5,32 L 12.5,32 L 14,29.5",
                "M 11,14 L 34,14",
            };
        #endregion
    }
}
