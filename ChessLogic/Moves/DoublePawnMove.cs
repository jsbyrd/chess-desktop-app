namespace ChessLogic
{
    class DoublePawnMove : Move
    {
        public override MoveType Type => MoveType.DoublePawn;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }
        private readonly Position SkippedPosition;

        public DoublePawnMove(Position fromPosition, Position toPosition)
        {
            FromPosition = fromPosition;
            ToPosition = toPosition;
            SkippedPosition = new Position((fromPosition.Row + toPosition.Row) / 2, fromPosition.Column);
        }

        public override bool MakeMove(Chessboard chessboard)
        {
            Player player = chessboard[FromPosition].Color;
            chessboard.SetEnPassantPosition(player, SkippedPosition);
            new NormalMove(FromPosition, ToPosition).MakeMove(chessboard);

            return true;
        }
    }
}
