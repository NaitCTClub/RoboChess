﻿using System;
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
    /**
    * @brief The pawn chess piece.
    **/
    public class King : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public King(Color pieceColor, Point id) : base(pieceColor, id) // Live
        {
            Img = GetPieceImage(TeamColor);
        }
        public King(GamePiece piece) : base(piece) { } // Virtual
        public King() { } // Used for Virtual Promotion

        public Image GetPieceImage(Color teamColor)
        {
            Image result;

            if (TeamColor == Color.Black)
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/BlackKing.png", UriKind.Relative))
                };
            //Img = ChessImages.Black_Queen;
            else
                result = new Image()
                {
                    Source = new BitmapImage(new Uri("Resources/whiteKing.png", UriKind.Relative))
                };

            return result;
        }

        public override List<BlindMove> BlindMoves()
        {
            List<BlindMove> blindMoves = new List<BlindMove>();
            //Rules for moving the king

            //Able to move one space in any direction on the board.
            //Can not move into a space that will allow it to be killed next turn.
            blindMoves.Add(new BlindMove(new Point(1, 0), 1));
            blindMoves.Add(new BlindMove(new Point(1, 1), 1));
            blindMoves.Add(new BlindMove(new Point(0, 1), 1));
            blindMoves.Add(new BlindMove(new Point(-1, 1), 1));
            blindMoves.Add(new BlindMove(new Point(-1, 0), 1));
            blindMoves.Add(new BlindMove(new Point(-1, -1), 1));
            blindMoves.Add(new BlindMove(new Point(0, -1), 1));
            blindMoves.Add(new BlindMove(new Point(1, -1), 1));
            if(this.moveCount == 0)
            {
                blindMoves.Add(new BlindMove(new Point(-2, 0), 1, Condition.Castling, new Point(this.ID.X - 4, this.ID.Y))); // Points to Rook
                blindMoves.Add(new BlindMove(new Point(2, 0), 1, Condition.Castling, new Point(this.ID.X + 3, this.ID.Y))); // Points to Rook
            }

            return blindMoves;
        }
    }
}