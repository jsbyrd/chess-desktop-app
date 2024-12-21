namespace ChessLogic
{
    public class GameState
    {
        public Chessboard Chessboard { get; }
        public Player CurrentPlayer { get; set; }
        public Result Result { get; set; } = null;

        public GameState(Player player, Chessboard chessboard)
        {
            CurrentPlayer = player;
            Chessboard = chessboard;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Chessboard.IsEmpty(pos) || Chessboard[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Chessboard[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Chessboard);
            return moveCandidates.Where(move => move.IsLegal(Chessboard));
        }

        public void MakeMove(Move move)
        {
            Chessboard.SetEnPassantPosition(CurrentPlayer, null);
            move.MakeMove(Chessboard);
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Chessboard.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Chessboard[pos];
                return piece.GetMoves(pos, Chessboard);
            });

            return moveCandidates.Where(move => move.IsLegal(Chessboard));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                // Checkmate (Decisive Result)
                if (Chessboard.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                // Draw (Stalemate)
                else
                {
                    Result = Result.Draw(EndState.Stalemate);
                }
            }
        }

        public bool IsGameOver()
        {
            return Result != null;
        }
    }
}
