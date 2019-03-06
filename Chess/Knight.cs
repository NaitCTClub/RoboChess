using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using ChessTools;

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
            if (TeamColor == Color.Black)
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/blackKnight.png", UriKind.Relative))
                };
            else
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteKnight.png", UriKind.Relative))
                };
        }        

        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();
            // Rules for moving Knight

            // Moves in strict L shape in any orientation
            // The Knight can jump over any other piece in the path
            // to its destination cell.
            // The L is made of two parts;
            // a) 2 steps in one direction 
            // b) 1 step perpendicular of step a)
            // Step a) & b) can be made in any order.
            blindMoves.Add(new BlindMove(new Point(-2, 1), 1));
            blindMoves.Add(new BlindMove(new Point(-1, 2), 1));
            blindMoves.Add(new BlindMove(new Point(1, 2), 1));
            blindMoves.Add(new BlindMove(new Point(2, 1), 1));
            blindMoves.Add(new BlindMove(new Point(2, -1), 1));
            blindMoves.Add(new BlindMove(new Point(1, -2), 1));
            blindMoves.Add(new BlindMove(new Point(-1, -2), 1));
            blindMoves.Add(new BlindMove(new Point(-2, -1), 1));

            return blindMoves;
        }
    }
}
