﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    /**
    * @brief The King chess piece.
    **/
    public class King : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        King(Color pieceColor, int id) : base(pieceColor, id)
        {

        }

        public override bool CanMove(Point destinationCell)
        {
            //Rules for moving the king

            //Able to move one space in any direction on the board.
            //Can not move into a space that will allow it to be killed next turn.
            return false;
        }
    }
}