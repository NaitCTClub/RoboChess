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
    public class Queen : GamePiece
    {
        /**
         * @brief Constructor for the Chess object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public Queen(Color pieceColor, Point id) : base(pieceColor, id)
        {
            if (TeamColor == Color.Black)
                Img = ChessImages.Black_Queen;
            else
                Img = ChessImages.White_Queen;
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
