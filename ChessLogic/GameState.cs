namespace ChessLogic
{
    public class GameState
    {
        public Chessboard Chessboard { get; }
        public Player CurrentPlayer { get; set; }
        public Result Result { get; set; } = null;
        private int fiftyMoveRuleCounter = 0;
        private string fenString;
        private readonly Dictionary<string, int> fenHistory = new Dictionary<string, int>();

        public GameState(Player player, Chessboard chessboard)
        {
            CurrentPlayer = player;
            Chessboard = chessboard;

            fenString = new FenString(CurrentPlayer, Chessboard).ToString();
            fenHistory[fenString] = 1;
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
            bool capturedOrPawn = move.MakeMove(Chessboard);
            if (capturedOrPawn)
            {
                fiftyMoveRuleCounter = 0;
                fenHistory.Clear();
            }
            else
            {
                fiftyMoveRuleCounter++;
            }

            CurrentPlayer = CurrentPlayer.Opponent();
            UpdateFenString();
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
            else if (Chessboard.IsInsufficientMaterial())
            {
                Result = Result.Draw(EndState.InsufficientMaterial);
            }
            else if (FiftyMoveRule())
            {
                Result = Result.Draw(EndState.FiftyMoveRule);
            }
            else if (ThreefoldRepetition())
            {
                Result = Result.Draw(EndState.ThreeFoldRepetition);
            }
        }

        public bool IsGameOver()
        {
            return Result != null;
        }

        private bool FiftyMoveRule()
        {
            // The reason for it being 100 is that a "move" is considered as both players making a move,
            // whereas each increment of our counter happens when either white or black makes a move
            return fiftyMoveRuleCounter == 100; 
        }

        private void UpdateFenString()
        {
            fenString = new FenString(CurrentPlayer, Chessboard).ToString();
            if (!fenHistory.TryGetValue(fenString, out int value)) fenHistory[fenString] = 1;
            else fenHistory[fenString] = ++value;
        }

        private bool ThreefoldRepetition()
        {
            return fenHistory[fenString] == 3;
        }
    }
}
