namespace Internet.Chess.Server.Fics
{
    public enum ChessPiece
    {
        [ServerVariableName("P")]
        Pawn,

        [ServerVariableName("N")]
        Knight,

        [ServerVariableName("R")]
        Rook,

        [ServerVariableName("B")]
        Bishop,

        [ServerVariableName("K")]
        King,

        [ServerVariableName("Q")]
        Queen,
    }
}
