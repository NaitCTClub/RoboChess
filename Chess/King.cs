﻿using System;
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
    public class King : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        public King(Color pieceColor, Point id) : base(pieceColor, id)
        {

        }
        
        public override bool[,] PossibleMove()
        {
            bool[,] result = new bool[8, 8];
            //Rules for moving the king

            //Able to move one space in any direction on the board.
            //Can not move into a space that will allow it to be killed next turn.
            return result;
        }
    }
}