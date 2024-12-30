namespace ChessLogic
{
    public static class RandomBot
    {
        public static void MakeRandomMove(GameState gameState)
        {
            Move[] moves = [.. gameState.AllLegalMovesFor(gameState.CurrentPlayer)];
            if (moves.Length == 0)
            {
                throw new Exception("No legal moves for RandomBot to execute");
            }

            Random random = new();
            int randomIndex = random.Next(0, moves.Length);
            Move randomlySelectedMove = moves[randomIndex];
            gameState.MakeMove(randomlySelectedMove);
        }
    }
}
