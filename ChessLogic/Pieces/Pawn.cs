using ChessLogic;

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
                if (oneRankPos.Row == 0 || oneRankPos.Row == 7)
                {
                    foreach (Move promotionMove in PromotionMoves(from, oneRankPos))
                    {
                        yield return promotionMove;
                    }
                }
                else
                {
                    yield return new NormalMove(from, oneRankPos);
                    Position twoRankPos = oneRankPos + Forward;

                    if (!HasMoved && CanMoveTo(twoRankPos, chessboard))
                    {
                        yield return new DoublePawnMove(from, twoRankPos);
                    }
                }

            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from, Chessboard chessboard)
        {
            foreach (Direction dir in new Direction[] {Direction.West, Direction.East })
            {
                Position to = from + Forward + dir;

                if (to == chessboard.GetEnPassantPosition(Color.Opponent()))
                {
                    yield return new EnPassantMove(from, to);
                }

                else if (CanCaptureAt(to, chessboard))
                {
                    if (to.Row == 0 || to.Row == 7)
                    {
                        foreach (Move promotionMove in PromotionMoves(from, to))
                        {
                            yield return promotionMove;
                        }
                    }
                    else
                    {
                        yield return new NormalMove(from, to);
                    }
                }
            }
        }

        private static IEnumerable<Move> PromotionMoves(Position from, Position to)
        {
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Queen);
        }

        public override bool CanCaptureOpponentKing(Position from, Chessboard chessboard)
        {
            return DiagonalMoves(from, chessboard).Any(move =>
            {
                Piece piece = chessboard[move.ToPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
