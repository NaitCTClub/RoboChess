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

        public override bool[,] PossibleMove()
        {
            bool[,] result = new bool[8, 8];
            // Rules for moving Rook

            // Able to move unlimited distance Horizontal or Vertically
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if((this.Location.X == x && this.Location.Y != y) || (this.Location.Y == y && this.Location.X != x))
                        result[x, y] = true;
                }

            }
                    return result;
        }
    }
}
