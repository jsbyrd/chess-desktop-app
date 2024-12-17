namespace ChessLogic
{
    public class Queen : Piece
    {
        public override PieceType Type { get; } = PieceType.Queen;
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

        public Queen(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Queen copy = new Queen(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            return MovePositionsInDirs(from, chessboard, dirs).Select(to => new NormalMove(from, to));
        }
    }
}
