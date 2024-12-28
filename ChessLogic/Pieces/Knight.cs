namespace ChessLogic
{
    public class Knight : Piece
    {
        public override PieceType Type { get; } = PieceType.Knight;
        public override Player Color { get; }

        public Knight(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Knight copy = new Knight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            return ValidMovePositions(from, chessboard).Select(to => new NormalMove(from, to));
        }

        private static IEnumerable<Position> PotentialToPositions(Position from)
        {
            foreach (Direction vDir in new Direction[] {Direction.North, Direction.South })
            {
                foreach (Direction hDir in new Direction[] { Direction.East, Direction.West })
                {
                    yield return from + (2 * vDir) + hDir;
                    yield return from + (2 * hDir) + vDir;
                }
            }
        }

        private IEnumerable<Position> ValidMovePositions(Position from, Chessboard chessboard)
        {
            return PotentialToPositions(from).Where(pos => Chessboard.IsInside(pos) && (chessboard.IsEmpty(pos) || chessboard[pos].Color != Color));
        }
    }
}
