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

        //public Player playerOne;
        //public Player playerTwo;
        //public Player WhosTurn;

        public MainWindow GUI;

        public Controller(MainWindow gui, Board brd)
        {
            GUI = gui;
            board = brd;
        }

        public void CellClick(Cell focusCell)
        {
            if (focusCell.Status == CellState.Default)
            {
                //Printing gamepiece for UI temporary effect
                focusCell.ToString();

                // Set cells status for moveable positions
                board.PossibleMoves(focusCell);

                board.ActiveCell = focusCell;

                GUI.RenameHeader("Choose target Cell");
            }
            // Move GamePiece from Active Cell to Focus Cell
            else
            {
                // MOVE,CAPTURE & TOGGLE TURN
                Board.Move move = board.GamePieceMove(board.ActiveCell, focusCell);

                // Notify next player's Turn
                GUI.RenameHeader($"Go {board.WhosTurn}");

                GUI.lbMoves.Items.Add($"{move.Piece} {move.From} To {move.To}");
            }
        }
    }
}


