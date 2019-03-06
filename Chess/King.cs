﻿using System;
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
    public class King : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public King(Color pieceColor, Point id) : base(pieceColor, id)
        {
            if (TeamColor == Color.Black)
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/blackKing.png", UriKind.Relative))
                };
            else
                Img = new System.Windows.Controls.Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteKing.png", UriKind.Relative))
                };
        }
        
        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();
            //Rules for moving the king

            //Able to move one space in any direction on the board.
            //Can not move into a space that will allow it to be killed next turn.
            blindMoves.Add(new BlindMove(new Point(1, 0), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(1, 1), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(0, 1), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(-1, 1), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(-1, 0), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(-1, -1), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(0, -1), 1, Cell.State.UnChecked));
            blindMoves.Add(new BlindMove(new Point(1, -1), 1, Cell.State.UnChecked));

            return blindMoves;
        }
    }
}