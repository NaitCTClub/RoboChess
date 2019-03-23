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
    public class Queen : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Queen(Color pieceColor, Point id) : base(pieceColor, id)
        {
            Img = GetPieceImage(TeamColor);
        }
        public Queen(GamePiece piece) : base(piece) { } // Virtual
        public Queen() { } // Used for Virtual Promotion

        public Image GetPieceImage(Color teamColor)
        {
            Image result;

            if (TeamColor == Color.Black)
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/BlackQueen.png", UriKind.Relative))
                };
            //Img = ChessImages.Black_Queen;
            else
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteQueen.png", UriKind.Relative))
                };

            return result;
        }

        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();
            //Rules for moving Queen

            // Able to move in any direction an unlimited amount of cells
            blindMoves.Add(new BlindMove(new Point(1, 0), -1));
            blindMoves.Add(new BlindMove(new Point(1, 1), -1));
            blindMoves.Add(new BlindMove(new Point(0, 1), -1));
            blindMoves.Add(new BlindMove(new Point(-1, 1), -1));
            blindMoves.Add(new BlindMove(new Point(-1, 0), -1));
            blindMoves.Add(new BlindMove(new Point(-1, -1), -1));
            blindMoves.Add(new BlindMove(new Point(0, -1), -1));
            blindMoves.Add(new BlindMove(new Point(1, -1), -1));

            return blindMoves;
        }
    }
}
