using System;
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
        public Pawn(Color pieceColor, Point id) : base(pieceColor, id)
        {

        }

        // Possible Moves, Blind to other Game Pieces
        public override List<Cell> PossibleMoves(Cell[,] board)
        {
            List<Cell> result = new List<Cell>();
            int vertical = 1;

            //Determine the movement direction based on color.
            if (PieceColor == Color.White)
                vertical *= -1;

            //By default a pawn can move forward one cell.
            if (Utils.InRange(Location.X + vertical, 0, Board.BOARD_SIZE-1))
                result.Add(board[Location.X + vertical, Location.X]);

            // Move 2 in the forward direction on first move
            if (Location == ID)
                if (Utils.InRange(Location.X + 2 * vertical, 0, Board.BOARD_SIZE-1))
                    result.Add(board[Location.X + 2 * vertical, Location.X]);

            // Forward Diagonal 1 position, only possible with opponent 
            // Piece in location
            if (Utils.InRange(Location.X + vertical, 0, Board.BOARD_SIZE-1))
            {
                if (Utils.InRange(Location.Y + 1, 0, Board.BOARD_SIZE - 1))
                    if (board[Location.X + vertical, Location.Y + 1].Piece != null)
                        result.Add(board[Location.X + vertical, Location.Y + 1]);

                if (Utils.InRange(Location.Y - 1, 0, Board.BOARD_SIZE - 1))
                    if (board[Location.X + vertical, Location.Y - 1].Piece != null)
                        result.Add(board[Location.X + vertical, Location.Y - 1]);
            }             

            return result;

        }
    }
}
