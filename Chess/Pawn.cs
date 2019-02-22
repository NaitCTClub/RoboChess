using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

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
            if (TeamColor == Color.Black)
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/blackPawn.png", UriKind.Relative))
                };
            else
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whitePawn.png", UriKind.Relative))
                };
        }

        // Possible Moves, Blind to other Game Pieces
        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();

            Point direction = new Point(0,0);

            if (this.TeamColor == Color.White)
                direction.Y = -1;
            else
                direction.Y = +1;


            // Rules for moving Pawn
            // Neutral move

            // Move 2 in the forward direction on first move
            if(this.Location == this.ID)
                blindMoves.Add(new BlindMove(direction, 2, Cell.State.Neutral));
            // Move 1 in the forward direcion thereafter
            else
                blindMoves.Add(new BlindMove(direction, 1, Cell.State.Neutral));
            // Attack move

            // Forward Diagonal 1 position, only possible with opponent 
            // Piece in location
            direction.X = 1;
            blindMoves.Add(new BlindMove(direction, 1, Cell.State.Enemy));
            direction.X = -1;
            blindMoves.Add(new BlindMove(direction, 1, Cell.State.Enemy));

            return blindMoves;
        }
    }
}
