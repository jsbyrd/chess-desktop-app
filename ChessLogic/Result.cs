namespace ChessLogic
{
    public class Result
    {
        public Player Winner { get; }
        public EndState Reason { get; }

        public Result(Player winner, EndState endState)
        {
            Winner = winner;
            Reason = endState;
        }

        public static Result Win(Player winner)
        {
            return new Result(winner, EndState.Checkmate);
        }

        public static Result Draw(EndState reason)
        {
            return new Result(Player.None, reason);
        }
    }
}
