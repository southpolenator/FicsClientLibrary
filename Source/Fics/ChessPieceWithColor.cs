namespace Internet.Chess.Server.Fics
{
    using System;

    public class ChessPieceWithColor : IEquatable<ChessPieceWithColor>
    {
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
