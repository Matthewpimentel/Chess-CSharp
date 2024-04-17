namespace ChessLogic
{
    public class Counting
    {
        private readonly Dictionary<PieceType, int> whiteCount = new();
        private readonly Dictionary<PieceType, int> blackCount = new();
        private int totalValue = 40;
        public int TotalCount { get; private set; }

        public Counting() 
        {
            foreach(PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                whiteCount[type] = 0;
                blackCount[type] = 0;
            }
        }

        public void Increment(Player color, PieceType type)
        {
            if(color == Player.White)
            {
                whiteCount[type]++;
            }
            else
            {
                blackCount[type]++;
            }

            TotalCount++;
        }

        public int White(PieceType type)
        {
            return whiteCount[type];
        }

        public int Black(PieceType type) 
        { 
            return blackCount[type]; 
        }

        public int WhiteValue()
        {
            int value = 0;
            foreach (var kvp in blackCount)
            {
                value += kvp.Value * PieceValue(kvp.Key);
            }
            return totalValue - value;
        }

        public int BlackValue()
        {
            int value = 0;
            foreach (var kvp in whiteCount)
            {
                value += kvp.Value * PieceValue(kvp.Key);
            }
            return totalValue - value;
        }

        private int PieceValue(PieceType type)
        {
            // Define the value for each piece type
            switch (type)
            {
                case PieceType.Pawn:
                    return 1;
                case PieceType.Rook:
                    return 5;
                case PieceType.Knight:
                case PieceType.Bishop:
                    return 3;
                case PieceType.Queen:
                    return 9;
                default:
                    return 1; // You might want to handle unexpected types differently
            }
        }
    }
}
