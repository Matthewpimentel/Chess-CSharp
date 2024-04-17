using ChessLogic;
using ChessLogic.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessAI
{
    public class AI
    {
        public Move GiveMove(GameState gameState)
        {
            Move bestMove = calcBestMove(gameState);
            return bestMove;
        }

        public static int EvaluateBoard(Board board, Player color)
        {
            // Sets the value for each piece using standard piece value
            Dictionary<PieceType, int> pieceValue = new Dictionary<PieceType, int>
        {
            {PieceType.Pawn, 100},
            {PieceType.Knight, 350},
            {PieceType.Bishop, 350},
            {PieceType.Rook, 525},
            {PieceType.Queen, 1000},
            {PieceType.King, 10000}
        };

            // Loop through all pieces on the board and sum up total
            int value = 0;
            foreach (var pos in board.PiecePositions())
            {
                Piece piece = board[pos];
                if (piece != null)
                {
                    // Subtract piece value if it is opponent's piece
                    int pieceVal = pieceValue[piece.Type];
                    value += pieceVal * (piece.Color == color ? 1 : -1);
                }
            }

            return value;
        }

        public Move calcBestMove(GameState gameState)
        {
            Random random = new Random();
            IEnumerable<Move> possibleMoves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            possibleMoves = possibleMoves.OrderBy(move => random.Next());

            if (gameState.IsGameOver() == true || possibleMoves.Count() == 0) return null;

            Move bestMoveSoFar = null;
            double bestMoveValue = double.NegativeInfinity;
            foreach (var move in possibleMoves)
            {
               Board copy = gameState.Board.Copy();
                new NormalMove(move.FromPos, move.ToPos).Execute(copy);
                int moveValue = EvaluateBoard(copy, Player.Black);
                if(moveValue > bestMoveValue)
                {
                    bestMoveSoFar = move;
                    bestMoveValue = moveValue;
                }
            }
            return bestMoveSoFar;
        }
    }
}
