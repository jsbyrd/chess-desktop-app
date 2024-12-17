namespace ChessLogic
{
    public class King : Piece
    {
        public override PieceType Type { get; } = PieceType.King;
        public override Player Color { get; }
        private static readonly Direction[] dirs =
        [
            Direction.North,
            Direction.NorthEast,
            Direction.East,
            Direction.SouthEast,
            Direction.South,
            Direction.SouthWest,
            Direction.West,
            Direction.NorthWest,
        ];

        public King(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            King copy = new King(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            foreach (Position to in ValidMovePositions(from, chessboard))
            {
                yield return new NormalMove(from, to);
            }
        }

        private IEnumerable<Position> ValidMovePositions(Position from, Chessboard chessboard)
        {
            foreach (Direction dir in dirs)
            {
                Position to = from + dir;

                if (!Chessboard.IsInside(to)) continue;

                if (chessboard.IsEmpty(to) || chessboard[to].Color != Color) yield return to;
            }
        }
    }
}
