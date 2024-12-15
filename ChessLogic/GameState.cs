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
    }
}
