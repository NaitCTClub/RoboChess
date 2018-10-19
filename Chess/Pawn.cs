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
        public Pawn(Color pieceColor, Point id) : base(pieceColor, id)
        {
              
        }

        // Possible Moves, Blind to other Game Pieces
        public override List<Point> PossibleMoves()
        {
            List<Point> result = new List<Point>();
            int vertical;

            if (this.PieceColor == Color.White)
                vertical = -1;
            else
                vertical = +1;


            // Rules for moving Pawn
            // Neutral move

            // Move 2 in the forward direction on first move
            if(this.Location == this.ID)
            {
                result[this.Location.X, InRange(this.Location.Y + vertical)] = true;
                result[this.Location.X, InRange(this.Location.Y + 2 * vertical)] = true;
            }
            // Move 1 in the forward direcion thereafter
            else
                result[this.Location.X, InRange(this.Location.Y + vertical)] = true;
            // Attack move

            // Forward Diagonal 1 position, only possible with opponent 
            // Piece in location
            result[InRange(this.Location.X + 1), InRange(this.Location.Y + vertical)] = true;
            result[InRange(this.Location.X - 1), InRange(this.Location.Y + vertical)] = true;

            return result;

        }

        // Prevents going out of Board's border
        private int InRange(int value)
        {
            int max = 7;
            int min = 0;
            int result = Math.Max(Math.Min(value, max), min);
            return result;
        }

    }
}
