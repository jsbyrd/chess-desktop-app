namespace ChessLogic
{
    public class Bishop : Piece
    {
        public override PieceType Type { get; } = PieceType.Bishop;
        public override Player Color { get; }
        private static readonly Direction[] dirs =
        [
            Direction.NorthEast,
            Direction.SouthEast,
            Direction.SouthWest,
            Direction.NorthWest,
        ];

        public Bishop(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Bishop copy = new Bishop(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            return MovePositionsInDirs(from, chessboard, dirs).Select(to => new NormalMove(from, to));
        }
    }
}
