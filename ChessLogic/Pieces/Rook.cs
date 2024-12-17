namespace ChessLogic
{
    public class Rook : Piece
    {
        public override PieceType Type { get; } = PieceType.Rook;
        public override Player Color { get; }
        private static readonly Direction[] dirs =
        [
            Direction.North,
            Direction.East,
            Direction.South,
            Direction.West,
        ];

        public Rook(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Rook copy = new Rook(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            return MovePositionsInDirs(from, chessboard, dirs).Select(to => new NormalMove(from, to));
        }
    }
}
