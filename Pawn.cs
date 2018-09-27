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
    public class Pawn : GamePiece
    {
        /**
         * @brief Constructor for the Pawn object. Calls the constructor for the base GamePiece
         * class before it does it's own construction.
         **/
        Pawn(Color pieceColor, int id) : base(pieceColor, id)
        {
              
        }

        
        public override void Move()
        {
            // Friendly moving
            //////////////////////
            // Move 2 on first move
            // Move 1 thereafter

            // Attack moving
            //////////////////////
            // Forward Diagonal 1 position, only possible with opponent 
            // Piece in location

        }

    }
}
