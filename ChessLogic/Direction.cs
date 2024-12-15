namespace ChessLogic
{
    // Assumes (0, 0) is a8 on the chessboard
    public class Direction
    {
        public readonly static Direction North = new Direction(-1, 0);
        public readonly static Direction South = new Direction(1, 0);
        public readonly static Direction East = new Direction(0, 1);
        public readonly static Direction West = new Direction(0, -1);
        public readonly static Direction NorthEast = North + East;
        public readonly static Direction NorthWest = North + West;
        public readonly static Direction SouthEast = South + East;
        public readonly static Direction SouthWest = South + West;
        public int RowDelta { get; }
        public int ColDelta { get; }

        public Direction(int rowDelta, int colDelta)
        {
            this.RowDelta = rowDelta;
            this.ColDelta = colDelta;
        }

        public static Direction operator +(Direction dir1, Direction dir2)
        {
            return new Direction(dir1.RowDelta + dir2.RowDelta, dir1.ColDelta + dir2.ColDelta);
        }

        public static Direction operator *(int scalar, Direction dir)
        {
            return new Direction(dir.RowDelta * scalar, dir.ColDelta * scalar);
        }
    }
}
