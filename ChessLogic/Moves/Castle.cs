namespace ChessLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; }
        public override Position FromPos {  get; }
        public override Position ToPos { get; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public Castle(MoveType type, Position kingpos)
        {
            Type = type;
            FromPos = kingpos;

            if(type == MoveType.CastleKS)
            {
                kingMoveDir = Direction.East;
                ToPos = new Position(kingpos.Row, 6);
                rookFromPos = new Position(kingpos.Row, 7);
                rookToPos = new Position(kingpos.Row, 5);
            }
            else if (type == MoveType.CastleQS)
            {
                kingMoveDir = Direction.West;
                ToPos = new Position(kingpos.Row, 2);
                rookFromPos = new Position(kingpos.Row, 0);
                rookToPos = new Position(kingpos.Row, 3);
            }
        }

        public override bool Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            new NormalMove(rookFromPos, rookToPos).Execute(board);

            return false;
        }

        public override bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;

            if (board.IsInCheck(player))
            {
                return false;
            }

            Board copy = board.Copy();
            Position kingPosInCopy = FromPos;

            for (int i = 0; i < 2; i++) 
            {
                new NormalMove(kingPosInCopy, kingPosInCopy + kingMoveDir).Execute(copy);
                kingPosInCopy += kingMoveDir;

                if (copy.IsInCheck(player))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
