using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    public class Knight : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Knight(Color pieceColor, Point id) : base(pieceColor, id)
        {
            
        }        

        public override bool[,] PossibleMove()
        {
            bool[,] result = new bool[8, 8];
            // Rules for moving Knight

            // Moves in strict L shape in any orientation
            // The Knight can jump over any other piece in the path
            // to its destination cell.
            // The L is made of two parts;
            // a) 2 steps in one direction 
            // b) 1 step perpendicular of step a)
            // Step a) & b) can be made in any order.

            return result;
        }
    }
}
