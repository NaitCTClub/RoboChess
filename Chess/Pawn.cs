using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    /**
    * @brief The pawn chess piece.
    **/
    public class Pawn : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        Pawn(Color pieceColor, int id) : base(pieceColor, id)
        {
              
        }

        
        public override bool CanMove(Point destinationCell)
        {

            // Rules for moving Pawn
            // Neutral move

            // Move 2 in the forward direction on first move
            // Move 1 in the forward direcion thereafter

            // Attack move

            // Forward Diagonal 1 position, only possible with opponent 
            // Piece in location

            return false;

        }

    }
}
