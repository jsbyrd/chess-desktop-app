namespace ChessLogic
{
    public class Chessboard
    {
        private readonly Piece[,] pieces = new Piece[8, 8];
        private readonly Dictionary<Player, Position> EnPassantPositions = new Dictionary<Player, Position>
        {
            {Player.White, null },
            {Player.Black, null }
        };

        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public static Chessboard Initial()
        {
            Chessboard chessboard = new Chessboard();
            chessboard.AddStartPieces();
            return chessboard;
        }

        private void AddStartPieces()
        {
            // Black Pieces
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            // White Pieces
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            // Pawns
            for (int col = 0; col < 8; col++)
            {
                this[1, col] = new Pawn(Player.Black);
                this[6, col] = new Pawn(Player.White);
            }
        }

        /// <summary>
        /// Returns true if position is contained within a normal chessboard, false otherwise.
        /// </summary>
        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        /// <summary>
        /// Returns true if no piece is occupying the given position, false otherwise.
        /// </summary>
        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> AllPiecePositions()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Position pos = new Position(row, col);
                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return AllPiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Chessboard Copy()
        {
            Chessboard copy = new Chessboard();

            foreach (Position pos in AllPiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }

            return copy;
        }

        public Position GetEnPassantPosition(Player player)
        {
            return EnPassantPositions[player];
        }

        public void SetEnPassantPosition(Player player, Position pos)
        {
            EnPassantPositions[player] = pos;
        }

        public PieceCounts CountPieces()
        {
            PieceCounts pieceCounts = new PieceCounts();

            foreach (Position pos in AllPiecePositions())
            {
                Piece piece = this[pos];
                pieceCounts.IncrementCount(piece.Color, piece.Type);
            }

            return pieceCounts;
        }

        public bool IsInsufficientMaterial()
        {
            PieceCounts pieceCounts = CountPieces();
            return IsKingVsKing(pieceCounts)
                || IsKingVsKingBishop(pieceCounts)
                || IsKingVsKingKnight(pieceCounts)
                || IsKingBishopVsKingBishop(pieceCounts);
        }

        private static bool IsKingVsKing(PieceCounts pieceCounts)
        {
            return pieceCounts.TotalCount == 2;
        }

        private static bool IsKingVsKingBishop(PieceCounts pieceCounts)
        {
            return pieceCounts.TotalCount == 3
                && (pieceCounts.WhitePieceCount(PieceType.Bishop) == 1 || pieceCounts.BlackPieceCount(PieceType.Bishop) == 1);
        }

        private static bool IsKingVsKingKnight(PieceCounts pieceCounts)
        {
            return pieceCounts.TotalCount == 3
                && (pieceCounts.WhitePieceCount(PieceType.Knight) == 1 || pieceCounts.BlackPieceCount(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVsKingBishop(PieceCounts pieceCounts)
        {
            if (pieceCounts.TotalCount != 4) return false;
            if (pieceCounts.WhitePieceCount(PieceType.Bishop) != 1 || pieceCounts.BlackPieceCount(PieceType.Bishop) != 1) return false;

            // Here, we have exactly 4 pieces: wKing, bKing, wBishop, and bBishop
            // Insufficient material only applies when the two bishops are opposite colored bishops
            Position whiteBishop = FindPiece(Player.White, PieceType.Bishop);
            Position blackBishop = FindPiece(Player.Black, PieceType.Bishop);
            return whiteBishop.GetSquareColor() == blackBishop.GetSquareColor();
        }

        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);
        }

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if (IsEmpty(kingPos) || IsEmpty(rookPos)) return false;

            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            return king.Type == PieceType.King
                && rook.Type == PieceType.Rook
                && !king.HasMoved
                && !rook.HasMoved;
        }

        public bool CanCastleKingSide(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }
        public bool CanCastleQueenSide(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position enPassantPos)
        {
            foreach (Position pos in pawnPositions.Where(IsInside))
            {
                Piece piece = this[pos];
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn) continue;

                EnPassantMove move = new EnPassantMove(pos, enPassantPos);

                if (move.IsLegal(this)) return true;
            }

            return false;
        }

        public bool CanCaptureEnPassant(Player player)
        {
            Position enPassantPos = GetEnPassantPosition(player.Opponent());

            if (enPassantPos == null) return false;

            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { enPassantPos + Direction.SouthWest, enPassantPos + Direction.SouthEast },
                Player.Black => new Position[] { enPassantPos + Direction.NorthWest, enPassantPos + Direction.NorthEast },
                _ => []
            };

            return HasPawnInPosition(player, pawnPositions, enPassantPos);
        }
    }
}
