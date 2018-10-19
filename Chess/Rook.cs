using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    public class Rook : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Rook(Color pieceColor, Point id) : base(pieceColor, id)
        {
            
        }

        public override List<Cell> PossibleMoves(Cell[,] board)
        {
            List<Cell> result = new List<Cell>();
            // Rules for moving Rook

            // Able to move unlimited distance Horizontal or Vertically
            for (int y = 0; y < Board.BOARD_SIZE; y++)
                for (int x = 0; x < Board.BOARD_SIZE; x++)
                    if ((Location.X == x && Location.Y != y) || (Location.Y == y && Location.X != x))
                        result.Add(board[x, y]);

            return result;
        }        
    }
}
