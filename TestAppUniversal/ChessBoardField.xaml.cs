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
        private bool white;
        private bool marked;
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

        public bool Marked
        {
            get { return marked; }
            set { UpdateField(ref marked, value); }
        }

        public ChessPieceWithColor ChessPiece
        {
            get { return chessPiece; }
            set { UpdateField(ref chessPiece, value); }
        }

        private void UpdateField<T>(ref T variable, T value) where T : IEquatable<T>
        {
            if ((value != null) != (variable != null) || (value != null && !value.Equals(variable)))
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
            get { return ChessPiece.Color == ChessPieceColor.White; }
        }

        private void UpdateLook()
        {
            LayoutRoot.Background = ChessBoardGraphics.DrawField(White);

            Canvas.Children.Clear();
            if (ChessPiece != null)
            {
                ChessPieceGraphics.DrawPiece(Canvas, ChessPiece);
            }

            if (marked)
            {
                ChessBoardGraphics.DrawFieldMark(Canvas);
            }
        }
    }
}
