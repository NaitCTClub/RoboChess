using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing;
using Color = System.Drawing.Color;

namespace Chess
{
    /**
     * @brief The class that all game pieces are based off of.
     **/
    public abstract class GamePiece
    {
        // Specific Instruction for blind moves for game piece
        public struct BlindMove
        {
            public Point Direction;
            public int Limit;
            public Cell.State Condition;
            public BlindMove(Point dir, int limit, Cell.State condition = Cell.State.Default)
            {
                Direction = dir;
                Limit = limit; // -1 = unlimited
                Condition = condition;
            }
        }

        public Color TeamColor { get; protected set; } //The color of the Team 
        public Point ID { get; protected set; }  //Identifier & starting location for the piece
        public Point Location { get; set; } //Current location
        public bool isAlive { get; set; } //Indicates if the piece is still active on the board.
        protected Point pos;
        public System.Windows.Controls.Image Img { get; set; }

        //Constructor for the game piece object. Initialize all parameters.
        protected GamePiece(Color pieceColor, Point id)
        {
            //Set the internal members from the passed in values.
            TeamColor = pieceColor;

            ID = id; //Default Location
            Location = id; //Current Location
            //All pieces start out as active.
            isAlive = true;
        }

        public abstract List<BlindMove> BlindMoves();

        public static GamePiece StartingPiece(Point cell)
        {
            Color color = Color.Red;

            GamePiece temp = null;

            //Top 2 Rows
            //BLACK PIECES
            if (cell.Y < 2)
            {
                color = Color.Black;
            }
            //Bottom 2 Rows
            //WHITE PIECES
            else if (cell.Y > 5)
            {
                color = Color.White;
            }
            else
                return null;

            //Pawn
            if (cell.Y == 1 || cell.Y == 6)
            {
                temp = new Pawn(color, cell);
            }
            //Rook
            else if (cell.X == 0 || cell.X == 7)
            {
                temp = new Rook(color, cell);
            }
            //Knight
            else if (cell.X == 1 || cell.X == 6)
            {
                temp = new Knight(color, cell);
            }
            //Bishop
            else if (cell.X == 2 || cell.X == 5)
            {
                temp = new Bishop(color, cell);
            }
            //Queen
            else if (cell.X == 3)
            {
                temp = new Queen(color, cell);
            }
            //King
            else if (cell.X == 4)
            {
                temp = new King(color, cell);
            }

            return temp;
        }
    }


}
