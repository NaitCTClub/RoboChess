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
        Queen(Color pieceColor, int id) : base(pieceColor, id)
        {
            
        }

        

        public override bool CanMove(Point destinationCell)
        {
            //Rules for moving Queen

            // Able to move in any direction an unlimited amount of cells

            return false;
        }
    }
}
