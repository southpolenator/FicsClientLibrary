using Internet.Chess.Server.Fics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TestAppUniversal
{
    public sealed partial class ChessBoard : UserControl
    {
        public const int OriginalBoardRows = 8;
        public const int OriginalBoardColumns = 8;
        private int rows;
        private int columns;
        private bool flipped;
        private ChessBoardField[,] fields;

        public ChessBoard()
        {
            InitializeComponent();
            Rows = OriginalBoardRows;
            Columns = OriginalBoardColumns;
        }

        public int Rows
        {
            get { return rows; }
            set { UpdateField(ref rows, value); }
        }

        public int Columns
        {
            get { return columns; }
            set { UpdateField(ref columns, value); }
        }

        public bool Flipped
        {
            get { return flipped; }
            set { UpdateField(ref flipped, value); }
        }

        public ChessPieceWithColor this[int row, int column]
        {
            get
            {
                return GetField(row, column).ChessPiece;
            }

            set
            {
                GetField(row, column).ChessPiece = value;
            }
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

        private void UpdateLook()
        {
            this.LayoutRoot.Children.Clear();
            this.LayoutRoot.RowDefinitions.Clear();
            this.LayoutRoot.ColumnDefinitions.Clear();
            for (int x = 0; x < this.Columns + 2; x++)
            {
                int size = x > 0 && x <= this.Columns ? 2 : 1;

                this.LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(size, GridUnitType.Star) });
            }

            for (int y = 0; y < this.Rows + 2; y++)
            {
                int size = y > 0 && y <= this.Rows ? 2 : 1;

                this.LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(size, GridUnitType.Star) });
            }

            for (int x = 0; x < this.Columns; x++)
            {
                for (int i = 0; i < 2; i++)
                {
                    TextBlock label = new TextBlock();

                    label.Text = ((char)('A' + x)).ToString();
                    label.TextAlignment = TextAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    PositionObject(i == 0 ? 0 : this.Rows + 1, x + 1, label);
                    this.LayoutRoot.Children.Add(label);
                }
            }

            for (int y = 0; y < this.Rows; y++)
            {
                for (int i = 0; i < 2; i++)
                {
                    TextBlock label = new TextBlock();

                    label.Text = (8 - y).ToString();
                    label.TextAlignment = TextAlignment.Center;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    PositionObject(y + 1, i == 0 ? 0 : this.Columns + 1, label);
                    this.LayoutRoot.Children.Add(label);
                }
            }

            fields = new ChessBoardField[Rows, Columns];
            for (int y = 0; y < this.Rows; y++)
            {
                for (int x = 0; x < this.Columns; x++)
                {
                    ChessBoardField field = new ChessBoardField();

                    field.White = (y + x) % 2 == 0;
                    PositionObject(y + 1, x + 1, field);
                    this.LayoutRoot.Children.Add(field);
                    fields[y, x] = field;
                }
            }
        }

        private ChessBoardField GetField(int row, int column)
        {
            return fields[row, column];
        }

        private void PositionObject(int row, int column, FrameworkElement element)
        {
            if (this.Flipped)
            {
                row = this.Rows + 1 - row;
                column = this.Columns + 1 - column;
            }

            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }
    }
}
