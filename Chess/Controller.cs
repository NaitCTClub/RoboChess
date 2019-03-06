using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessTools;

namespace Chess
{
    public class Controller
    {
        public Board board = new Board();
        public Point ActiveCell; // Points to selected cell in [8,8], coordinates for nothing selected ->(-1, -1)  
        public Point TargetCell;
        public List<GamePiece> blackDead;
        public List<GamePiece> whiteDead;
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;


        public void CellClick(int x, int y)
        {
            /// Find possible Moves for Game Piece
            //board.SelectCell(x, y);
            /// Exit if Cell is Not Owned by Player
            //if (ActiveCell != new Point(-1, -1))
               // return;
            //board.CanMove(board.ActiveCell);
            //board.HighlightCells(board.SelectCell(x, y));
            //int[,] temp = new int[8, 8];
            //temp = board.CanMove(ActiveCell);

        }
    }
}


