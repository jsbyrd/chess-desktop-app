namespace ChessLogic
{
    public class NormalMove : Move
    {
        public override MoveType Type { get; } = MoveType.Normal;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }
        public NormalMove(Position from, Position to)
        {
            FromPosition = from;
            ToPosition = to;
        }

        public override bool MakeMove(Chessboard chessboard)
        {
            Piece piece = chessboard[FromPosition];
            bool hasCapturedPiece = !chessboard.IsEmpty(ToPosition);
            chessboard[ToPosition] = piece;
            chessboard[FromPosition] = null;
            piece.HasMoved = true;

            return hasCapturedPiece || piece.Type == PieceType.Pawn;
        }
    }
}
