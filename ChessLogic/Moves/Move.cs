namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPosition { get; }
        public abstract Position ToPosition { get; }
        public abstract void MakeMove(Chessboard chessboard);

        /// <summary>
        /// Checks to see if this move results in the current player's king in check
        /// </summary>
        /// <param name="board"></param>
        /// <returns>True if current player's king in check, False otherwise</returns>
        public virtual bool IsLegal(Chessboard chessboard)
        {
            Player movingPlayer = chessboard[FromPosition].Color;
            Chessboard copy = chessboard.Copy();
            MakeMove(copy);
            return !copy.IsInCheck(movingPlayer);
        }
    }
}
