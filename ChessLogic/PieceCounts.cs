namespace ChessLogic
{
    public class PieceCounts
    {
        private readonly Dictionary<PieceType, int> WhiteCount = new();
        private readonly Dictionary<PieceType, int> BlackCount = new();
        public int TotalCount { get; set; }

        public PieceCounts()
        {
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                WhiteCount[type] = 0;
                BlackCount[type] = 0;
            }
        }

        public void IncrementCount(Player color, PieceType type)
        {
            if (color == Player.White)
            {
                WhiteCount[type] += 1;
            }
            else if (color == Player.Black)
            {
                BlackCount[type] += 1;
            }
            TotalCount += 1;
        }

        public int WhitePieceCount(PieceType type)
        {
            return WhiteCount[type];
        }

        public int BlackPieceCount(PieceType type)
        {
            return BlackCount[type];
        }
    }
}
