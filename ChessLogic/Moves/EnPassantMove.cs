namespace ChessLogic.Moves
{
    public class EnPassantMove : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }
        private readonly Position CapturedPawnPosition;

        public EnPassantMove(Position from, Position to)
        {
            FromPosition = from;
            ToPosition = to;
            CapturedPawnPosition = new Position(from.Row, to.Column);
        }

        public override void MakeMove(Chessboard chessboard)
        {
            new NormalMove(FromPosition, ToPosition).MakeMove(chessboard);
            chessboard[CapturedPawnPosition] = null;
        }
    }
}
