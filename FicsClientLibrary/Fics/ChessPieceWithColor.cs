namespace Internet.Chess.Server.Fics
{
    public class ChessPieceWithColor
    {
        public ChessPiece Piece { get; set; }
        public ChessPieceColor Color { get; set; }

        public override string ToString()
        {
            return Color.ToString() + " " + Piece.ToString();
        }
    }
}
