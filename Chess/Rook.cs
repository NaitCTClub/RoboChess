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
        Rook(Color pieceColor, int id) : base(pieceColor, id)
        {
            
        }

        

        public override bool CanMove(Point destinationCell)
        {
            //Able to move unlimited distance Horizontal or Vertically
            return false;
            //
        }
    }
}
