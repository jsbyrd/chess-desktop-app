using System.Drawing;

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

            if (CanCastleKingSide(from, chessboard))
            {
                yield return new CastleMove(MoveType.CastleKS, from);
            }

            if (CanCastleQueenSide(from, chessboard))
            {
                yield return new CastleMove(MoveType.CastleQS, from);
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

        public override bool CanCaptureOpponentKing(Position from, Chessboard chessboard)
        {
            return ValidMovePositions(from, chessboard).Any(to =>
            {
                Piece piece = chessboard[to];
                return piece != null && piece.Type == PieceType.King;
            });
        }

        private bool IsValidUnmovedRook(Position pos, Chessboard chessboard)
        {
            if (chessboard.IsEmpty(pos)) return false;

            Piece piece = chessboard[pos];

            return piece.Type == PieceType.Rook && !piece.HasMoved && piece.Color == Color;
        }

        private static bool AllEmpty(IEnumerable<Position> positions, Chessboard chessboard)
        {
            return positions.All(pos => chessboard.IsEmpty(pos));
        }

        private bool CanCastleKingSide(Position from, Chessboard chessboard)
        {
            if (HasMoved) return false;

            Position rookPos = new Position(from.Row, 7);
            Position[] betweenPositions = [new(from.Row, 5), new Position(from.Row, 6)];

            return IsValidUnmovedRook(rookPos, chessboard) && AllEmpty(betweenPositions, chessboard);
        }

        private bool CanCastleQueenSide(Position from, Chessboard chessboard)
        {
            if (HasMoved) return false;

            Position rookPos = new Position(from.Row, 0);
            Position[] betweenPositions = [new(from.Row, 1), new Position(from.Row, 2), new Position(from.Row, 3)];

            return IsValidUnmovedRook(rookPos, chessboard) && AllEmpty(betweenPositions, chessboard);
        }
    }
}
