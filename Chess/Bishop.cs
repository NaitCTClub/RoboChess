using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    public class Bishop : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        Bishop(Color pieceColor, int id) : base(pieceColor, id)
        {
            
        }

        

        public override bool CanMove(Point destinationCell)
        {
            //Rules for Moving Bishop

            //Able to move unlimited distance Diagonally in any direction.
            return false;
        }
    }
}
