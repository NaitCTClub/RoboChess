using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Chess
{
    public class Rook : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Rook(Color pieceColor, Point id) : base(pieceColor, id)
        {
            if (TeamColor == Color.Black)
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/blackRook.png", UriKind.Relative))
                };
            else
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteRook.png", UriKind.Relative))
                };
        }        

        // Instructions for moving rook
        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();

            // Able to move unlimited distance Horizontal or Vertically
            blindMoves.Add(new BlindMove(new Point(1, 0), -1));
            blindMoves.Add(new BlindMove(new Point(-1, 0), -1));
            blindMoves.Add(new BlindMove(new Point(0, 1), -1));
            blindMoves.Add(new BlindMove(new Point(0, -1), -1));

            return blindMoves;
        }
    }
}
