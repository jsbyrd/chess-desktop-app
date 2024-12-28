namespace ChessLogic
{
    public class PawnPromotion : Move
    {
        public override MoveType Type => MoveType.PawnPromotion;
        public override Position FromPosition { get; }
        public override Position ToPosition { get; }
        private readonly PieceType NewType;
        
        public PawnPromotion(Position from, Position to, PieceType newType)
        {
            FromPosition = from;
            ToPosition = to;
            NewType = newType;
        }

        private Piece CreatePromotionPiece(Player color)
        {
            return NewType switch
            {
                PieceType.Knight => new Knight(color),
                PieceType.Bishop => new Bishop(color),
                PieceType.Rook => new Rook(color),
                PieceType.Queen => new Queen(color),
                _ => new Queen(color)
            };
        }

        public override bool MakeMove(Chessboard chessboard)
        {
            Piece pawn = chessboard[FromPosition];
            chessboard[FromPosition] = null;
            Piece promotionPiece = CreatePromotionPiece(pawn.Color);
            promotionPiece.HasMoved = true;
            chessboard[ToPosition] = promotionPiece;

            return true;
        }
    }
}
