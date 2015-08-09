namespace Internet.Chess.Server.Fics
{
    using System;

    public class ChessPieceWithColor : IEquatable<ChessPieceWithColor>
    {
        public static ChessPieceWithColor BlackBishop
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Bishop, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor BlackKing
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.King, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor BlackKnight
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Knight, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor BlackPawn
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Pawn, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor BlackQueen
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Queen, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor BlackRook
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Rook, Color = ChessPieceColor.Black }; }
        }

        public static ChessPieceWithColor WhiteBishop
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Bishop, Color = ChessPieceColor.White }; }
        }

        public static ChessPieceWithColor WhiteKing
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.King, Color = ChessPieceColor.White }; }
        }

        public static ChessPieceWithColor WhiteKnight
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Knight, Color = ChessPieceColor.White }; }
        }

        public static ChessPieceWithColor WhitePawn
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Pawn, Color = ChessPieceColor.White }; }
        }

        public static ChessPieceWithColor WhiteQueen
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Queen, Color = ChessPieceColor.White }; }
        }

        public static ChessPieceWithColor WhiteRook
        {
            get { return new ChessPieceWithColor() { Type = ChessPieceType.Rook, Color = ChessPieceColor.White }; }
        }

        public ChessPieceType Type { get; set; }
        public ChessPieceColor Color { get; set; }

        public override string ToString()
        {
            return Color.ToString() + " " + Type.ToString();
        }

        public bool Equals(ChessPieceWithColor other)
        {
            return other != null && other.Type == Type && other.Color == Color;
        }
    }
}
