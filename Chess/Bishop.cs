using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using ChessTools;
using Image = System.Windows.Controls.Image;

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
            Img = GetPieceImage(TeamColor);
        }
        public Bishop(GamePiece piece) : base(piece) { } // Virtual
        public Bishop() { } // Used for Virtual Promotion

        public Image GetPieceImage(Color teamColor)
        {
            Image result;

            if (TeamColor == Color.Black)
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/BlackBishop.png", UriKind.Relative))
                };
            //Img = ChessImages.Black_Queen;
            else
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteBishop.png", UriKind.Relative))
                };

            return result;
        }

        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();
            //Rules for Moving Bishop

            //Able to move unlimited distance Diagonally in any direction.
            blindMoves.Add(new BlindMove(new Point(1, 1), -1));
            blindMoves.Add(new BlindMove(new Point(-1, 1), -1));
            blindMoves.Add(new BlindMove(new Point(-1, -1), -1));
            blindMoves.Add(new BlindMove(new Point(1, -1), -1));

            return blindMoves;
        }
    }
}
