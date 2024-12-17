
namespace ChessLogic
{
    public class Pawn : Piece
    {
        public override PieceType Type { get; } = PieceType.Pawn;
        public override Player Color { get; }
        private readonly Direction Forward;
        
        public Pawn(Player color)
        {
            Color = color;

            if (Color == Player.White)
            {
                Forward = Direction.North;
            }
            else if (color == Player.Black)
            {
                Forward = Direction.South;
            }
        }

        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Chessboard chessboard)
        {
            return ForwardMoves(from, chessboard).Concat(DiagonalMoves(from, chessboard));
        }

        private static bool CanMoveTo(Position pos, Chessboard chessboard)
        {
            return Chessboard.IsInside(pos) && chessboard.IsEmpty(pos);
        }

        private bool CanCaptureAt(Position pos, Chessboard chessboard)
        {
            if (!Chessboard.IsInside(pos) || chessboard.IsEmpty(pos))
            {
                return false;
            }

            return chessboard[pos].Color != Color;
        }

        private IEnumerable<Move> ForwardMoves(Position from, Chessboard chessboard)
        {
            Position oneRankPos = from + Forward;
            
            if (CanMoveTo(oneRankPos, chessboard))
            {
                yield return new NormalMove(from, oneRankPos);
                Position twoRankPos = oneRankPos + Forward;

                if (!HasMoved && CanMoveTo(twoRankPos, chessboard))
                {
                    yield return new NormalMove(from, twoRankPos);
                }
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from, Chessboard chessboard)
        {
            foreach (Direction dir in new Direction[] {Direction.West, Direction.East })
            {
                Position to = from + Forward + dir;
                if (CanCaptureAt(to, chessboard))
                {
                    yield return new NormalMove(from, to);
                }
            }
        }
    }
}
