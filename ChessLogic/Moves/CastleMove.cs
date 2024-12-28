namespace ChessLogic
{
    public class CastleMove : Move
    {
        public override MoveType Type { get; }
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }
        private readonly Direction KingMoveDirection;
        private readonly Position RookFromPosition;
        private readonly Position RookToPosition;

        public CastleMove(MoveType type, Position kingPos)
        {
            Type = type;
            FromPosition = kingPos;

            if (type == MoveType.CastleKS)
            {
                KingMoveDirection = Direction.East;
                ToPosition = new Position(kingPos.Row, 6);
                RookFromPosition = new Position(kingPos.Row, 7);
                RookToPosition = new Position(kingPos.Row, 5);
            }
            else if (type == MoveType.CastleQS)
            {
                KingMoveDirection = Direction.West;
                ToPosition = new Position(kingPos.Row, 2);
                RookFromPosition = new Position(kingPos.Row, 0);
                RookToPosition = new Position(kingPos.Row, 3);
            }
        }

        public override bool MakeMove(Chessboard chessboard)
        {
            // Move King
            new NormalMove(FromPosition, ToPosition).MakeMove(chessboard);
            // Move Rook
            new NormalMove(RookFromPosition, RookToPosition).MakeMove(chessboard);

            return false;
        }

        public override bool IsLegal(Chessboard chessboard)
        {
            Player movingPlayer = chessboard[FromPosition].Color;

            // Can't castle while in check
            if (chessboard.IsInCheck(movingPlayer)) return false;

            // Can't castle while any square the king moves through "is in check"
            Chessboard copy = chessboard.Copy();
            Position kingPosInCopy = FromPosition;
            for (int i = 0; i < 2; i++)
            {
                new NormalMove(kingPosInCopy, kingPosInCopy + KingMoveDirection).MakeMove(copy);
                kingPosInCopy += KingMoveDirection;
                if (copy.IsInCheck(movingPlayer)) return false;
            }

            return true;
        }
    }
}
