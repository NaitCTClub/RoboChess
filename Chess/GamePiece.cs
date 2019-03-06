using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing;
using Color = System.Drawing.Color;
using ChessTools;

namespace Chess
{
    /**
     * @brief The class that all game pieces are based off of.
     **/
    public abstract class GamePiece
    {
        public Color TeamColor { get; protected set; } //The color of the Team 
        public Point ID { get; protected set; }  //Identifier & starting location for the piece
        public Point Location { get; set; } //Current location
        public bool isAlive { get; protected set; } //Indicates if the piece is still active on the board.
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
                Pawn temp = new Pawn(color, cell);
                return temp;
            }
            //Rook
            else if (cell.X == 0 || cell.X == 7)
            {
                Rook temp = new Rook(color, cell);
                return temp;
            }
            //Knight
            else if (cell.X == 1 || cell.X == 6)
            {
                Knight temp = new Knight(color, cell);
                return temp;
            }
            //Bishop
            else if (cell.X == 2 || cell.X == 5)
            {
                Bishop temp = new Bishop(color, cell);
                return temp;
            }
            //Queen
            else if (cell.X == 3)
            {
                Queen temp = new Queen(color, cell);
                return temp;
            }
            //King
            else if (cell.X == 4)
            {
                King temp = new King(color, cell);
                return temp;
            }

            else
                return null;
        }
    }


}
