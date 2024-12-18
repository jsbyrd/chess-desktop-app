namespace ChessLogic
{
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;
        public abstract Piece Copy();
        public abstract IEnumerable<Move> GetMoves(Position from, Chessboard board);

        /// <summary>
        /// Used for sliding moves (i.e. Queen, Rook, Bishop)
        /// </summary>
        /// <param name="from">The starting position of the piece</param>
        /// <param name="chessboard">The chessboard in play</param>
        /// <param name="dir">The direction of the sliding move</param>
        /// <returns>All valid sliding moves for a given direction</returns>
        protected IEnumerable<Position> MovePositionsInDir(Position from, Chessboard chessboard, Direction dir)
        {
            for (Position pos = from + dir; Chessboard.IsInside(pos); pos += dir)
            {
                // Legal empty square
                if (chessboard.IsEmpty(pos))
                {
                    yield return pos;
                    continue;
                }

                // Piece is occupying position
                Piece occupyingPiece = chessboard[pos];
                if (occupyingPiece.Color != Color)
                {
                    yield return pos;
                }

                yield break;
            }

        }
        protected IEnumerable<Position> MovePositionsInDirs(Position from, Chessboard chessboard, Direction[] dirs)
        {
            return dirs.SelectMany(dir => MovePositionsInDir(from, chessboard, dir));
        }

        public virtual bool CanCaptureOpponentKing(Position from, Chessboard chessboard)
        {
            return GetMoves(from, chessboard).Any(move =>
            {
                Piece piece = chessboard[move.ToPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
