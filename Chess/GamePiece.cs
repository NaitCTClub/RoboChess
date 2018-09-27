using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    /**
     * @brief The class that all game pieces are based off of.
     **/
    public abstract class GamePiece
    {
        public Color PieceColor { get; protected set; } //The color of the gamie piece.
        public int ID { get; protected set; }  //Identifier for the piece
        public bool isAlive { get; protected set; } //Indicates if the piece is still active on the board.

        //Constructor for the game piece object. Initialize all parameters.
        protected GamePiece(Color pieceColor, int id)
        {
            //Set the internal members from the passed in values.
            PieceColor = pieceColor;
            ID = id;
            //All pieces start out as active.
            isAlive = true;


        }

        public abstract void Move();
    }


}
