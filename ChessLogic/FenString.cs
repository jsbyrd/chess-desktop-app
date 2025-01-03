using System.Text;

namespace ChessLogic
{
    public class FenString
    {
        private readonly StringBuilder sb = new StringBuilder();

        public FenString(Player currentPlayer, Chessboard chessboard)
        {
            AddPiecePlacement(chessboard);
            sb.Append(' ');
            AddCurrentPlayer(currentPlayer);
            sb.Append(' ');
            AddCastlingRights(chessboard);
            sb.Append(' ');
            AddEnPassant(chessboard, currentPlayer);
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        private static char PieceChar(Piece piece)
        {
            char c = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Rook => 'r',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => ' ',
            };

            if (piece.Color == Player.White)
            {
                return char.ToUpper(c);
            }
            else
            {
                return c;
            }
        }

        private void AddRowData(Chessboard chessboard, int row)
        {
            int emptySpaces = 0;

            for (int col = 0; col < 8; col++)
            {
                if (chessboard[row, col] == null)
                {
                    emptySpaces++;
                    continue;
                }

                if (emptySpaces > 0)
                {
                    sb.Append(emptySpaces);
                    emptySpaces = 0;
                }

                sb.Append(PieceChar(chessboard[row, col]));
            }

            if (emptySpaces > 0) sb.Append(emptySpaces);
        }

        private void AddPiecePlacement(Chessboard chessboard)
        {
            for (int row = 0; row < 8; row++)
            {
                if (row != 0) sb.Append('/');
                AddRowData(chessboard, row);
            }
        }

        private void AddCurrentPlayer(Player currentPlayer)
        {
            if (currentPlayer == Player.White) sb.Append('w');
            else sb.Append('b');
        }

        private void AddCastlingRights(Chessboard chessboard)
        {
            bool castleWhiteKS = chessboard.CanCastleKingSide(Player.White);
            bool castleWhiteQS = chessboard.CanCastleQueenSide(Player.White);
            bool castleBlackKS = chessboard.CanCastleKingSide(Player.Black);
            bool castleBlackQS = chessboard.CanCastleQueenSide(Player.Black);

            if (!(castleWhiteKS || castleWhiteQS || castleBlackKS || castleBlackQS))
            {
                sb.Append('-');
                return;
            }

            if (castleWhiteKS) sb.Append('K');
            if (castleWhiteQS) sb.Append('Q');
            if (castleBlackKS) sb.Append('k');
            if (castleBlackQS) sb.Append('q');
        }

        private void AddEnPassant(Chessboard chessboard, Player currentPlayer)
        {
            if (!chessboard.CanCaptureEnPassant(currentPlayer))
            {
                sb.Append('-');
            }
            else
            {
                Position pos = chessboard.GetEnPassantPosition(currentPlayer.Opponent());
                char file = (char)('a' + pos.Column);
                int rank = 8 - pos.Column;
                sb.Append(file);
                sb.Append(rank);
            }
        }

        public static GameState GameStateFromFen(string fen)
        {
            string[] parts = fen.Split(' ');
            if (parts.Length < 4)
                throw new ArgumentException("Invalid FEN string");

            Chessboard chessboard = new Chessboard();
            string[] rows = parts[0].Split('/');
            for (int row = 0; row < 8; row++)
            {
                int col = 0;
                foreach (char c in rows[row]) // FEN starts from rank 8
                {
                    if (char.IsDigit(c))
                    {
                        col += c - '0';
                    }
                    else
                    {
                        Player player = Char.IsUpper(c) ? Player.White : Player.Black;
                        Piece piece = Char.ToLower(c) switch
                        {
                            'p' => new Pawn(player),
                            'n' => new Knight(player),
                            'b' => new Bishop(player),
                            'r' => new Rook(player),
                            'q' => new Queen(player),
                            'k' => new King(player),
                            _ => null,
                        };
                        chessboard[new Position(row, col)] = piece;
                        col++;
                    }
                }
            }

            var currentPlayer = parts[1] == "w" ? Player.White : Player.Black;

            chessboard.SetEnPassantPosition(Player.White, null);
            chessboard.SetEnPassantPosition(Player.Black, null);
            if (parts[3] != "-")
            {
                int file = parts[3][0] - 'a';
                int rank = '8' - parts[3][1];
                chessboard.SetEnPassantPosition(currentPlayer.Opponent(), new Position(rank, file));
            }

            return new GameState(currentPlayer, chessboard);
        }
    }
}
