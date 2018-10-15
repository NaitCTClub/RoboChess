using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    public class Queen : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Queen(Color pieceColor, Point id) : base(pieceColor, id)
        {
            
        }

        

        public override bool[,] PossibleMove()
        {
            bool[,] result = new bool[8, 8];
            //Rules for moving Queen

            // Able to move in any direction an unlimited amount of cells

            return result;
        }
    }
}
