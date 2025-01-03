using System.Diagnostics;

namespace ChessLogic
{
    public static class ChessEngineBot
    {
        public readonly static bool UseMoveOrdering = false;
        private readonly static int MaximumValue = 1000000;
            private readonly static int MinimumValue = -1000000;
        public static void MakeEngineMove(GameState gameState, int moveDepth)
        {
            Move bestMove = null;
            int bestValue = int.MinValue;

            Stopwatch stopwatch = new();

            stopwatch.Start();

            IEnumerable<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            if (UseMoveOrdering)
            {
                moves = OrderMoves(gameState, moves);
            }

            foreach (Move move in moves)
            {
                GameState newGameState = new GameState(gameState.CurrentPlayer, gameState.Chessboard.Copy());
                newGameState.MakeMove(move);
                int moveValue = Minimax(newGameState, moveDepth - 1, int.MinValue, int.MaxValue, false);


                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }

                if (moveValue == MinimumValue) break;
            }

            stopwatch.Stop();

            Trace.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalSeconds}s");

            if (bestMove == null)
            {
                throw new Exception("No legal moves for EngineBot to execute");
            }
            gameState.MakeMove(bestMove);
        }

        private static int Evaluate(GameState gameState, bool isMaximizingPlayer, int depth)
        {
            bool isGameOver = gameState.IsGameOver();
            if (isGameOver && gameState.Result?.Winner == Player.None) return 0;
            if (isGameOver && isMaximizingPlayer) return MinimumValue - depth;
            if (isGameOver && !isMaximizingPlayer) return MaximumValue + depth;

            int score = 0;
            foreach (Position pos in gameState.Chessboard.PiecePositionsFor(Player.White))
            {
                score += GetPieceValue(gameState.Chessboard[pos]);
            }
            foreach (Position pos in gameState.Chessboard.PiecePositionsFor(Player.Black))
            {
                score -= GetPieceValue(gameState.Chessboard[pos]);
            }
            return gameState.CurrentPlayer == Player.White ? score : -score;
        }

        private static int GetPieceValue(Piece piece)
        {
            return piece.Type switch
            {
                PieceType.Pawn => 100,
                PieceType.Knight => 300,
                PieceType.Bishop => 320, // For now, make Bishop slightly more valuable than Knight
                PieceType.Rook => 500,
                PieceType.Queen => 900,
                PieceType.King => 100000,
                _ => 0,
            };
        }

        // Used to make alpha-beta pruning more efficient
        private static IEnumerable<Move> OrderMoves(GameState gameState, IEnumerable<Move> moves)
        {
            return moves.OrderByDescending(move =>
            {
                Piece capturedPiece = gameState.Chessboard[move.ToPosition];
                if (capturedPiece != null && capturedPiece.Type == PieceType.King) throw new Exception("King has been illegally captured!");
                int captureValue = capturedPiece != null ? GetPieceValue(capturedPiece) : 0;
                int movingPieceValue = GetPieceValue(gameState.Chessboard[move.FromPosition]);
                return captureValue - movingPieceValue;
            });
        }

        private static int Minimax(GameState gameState, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || gameState.IsGameOver())
            {
                return Evaluate(gameState, maximizingPlayer, depth);
            }

            IEnumerable<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            if (UseMoveOrdering)
            {
                moves = OrderMoves(gameState, moves);
            }

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in moves)
                {
                    GameState newGameState = new GameState(gameState.CurrentPlayer, gameState.Chessboard.Copy());
                    newGameState.MakeMove(move);
                    int eval = Minimax(newGameState, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in moves)
                {
                    GameState newGameState = new GameState(gameState.CurrentPlayer, gameState.Chessboard.Copy());
                    newGameState.MakeMove(move);
                    int eval = Minimax(newGameState, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }
    }
}
