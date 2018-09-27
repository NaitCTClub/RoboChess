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
        King(Color pieceColor, int id) : base(pieceColor, id) { }
    }
}