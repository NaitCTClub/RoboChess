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
        public Bishop(Color pieceColor, Point id) : base(pieceColor, id)
        {
            
        }
    
      

        public override List<Point> PossibleMoves()
        {
            List<Point> result = new List<Point>();
            //Rules for Moving Bishop

            //Able to move unlimited distance Diagonally in any direction.
            return result;
        }
    }
}
