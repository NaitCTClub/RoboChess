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
        

        public override List<Cell> PossibleMoves(Cell[,] board)
        {
            List<Cell> result = new List<Cell>();
            //Rules for Moving Bishop

            //Able to move unlimited distance Diagonally in any direction.
            return result;
        }
    }
}