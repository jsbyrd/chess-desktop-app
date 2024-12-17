namespace ChessLogic
{
    public class GameState
    {
        public Chessboard Chessboard { get; }
        public Player CurrentPlayer { get; set; }

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
            return piece.GetMoves(pos, Chessboard);
        }

        public void MakeMove(Move move)
        {
            move.MakeMove(Chessboard);
            CurrentPlayer = CurrentPlayer.Opponent();
        }
    }
}
