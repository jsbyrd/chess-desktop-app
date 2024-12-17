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

        public override void MakeMove(Chessboard chessboard)
        {
            Piece piece = chessboard[FromPosition];
            chessboard[ToPosition] = piece;
            chessboard[FromPosition] = null;
            piece.HasMoved = true;
        }
    }
}
