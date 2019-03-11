using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessTools;
using static ChessTools.Library;

namespace Chess
{
    public class Controller
    {
        public Board LiveBoard = new Board();
        public List<ChessMove> Moves = new List<ChessMove>();
        public int Moves_Index = -1; // Points to the Move currently viewed on LiveBoard 

        public List<ChessMove> possibleMoves = new List<ChessMove>();

        //public Player playerOne;
        //public Player playerTwo;
        //public Player WhosTurn;

        public MainWindow GUI;

        public Controller(MainWindow gui, Board brd)
        {
            GUI = gui;
            LiveBoard = brd;
        }

        public void CellClick(Cell focusCell)
        {

            if (focusCell.Status == Condition.Active)
                return; // Clicking the currently selected cell does nothing

            //==============================================================================
            //                         Select GamePiece
            //==============================================================================
            if (focusCell.Status == Condition.Default)
            {
                LiveBoard.HighlightBoard();
                GamePiece piece = focusCell.Piece;

                // Get & Set cell status for possible moves
                possibleMoves = LiveBoard.PossibleMoves(piece);

                LiveBoard.HighlightBoard(possibleMoves);

                GUI.RenameHeader("Choose target Cell");
            }
            //==============================================================================
            //                          Move GamePiece
            //==============================================================================
            else
            {
                // MOVE,CAPTURE & TOGGLE TURN
                ChessMove move = possibleMoves.Find(moveFind => moveFind.To == focusCell);

                LiveBoard.Move_GamePiece(move);

                ArchiveMove(move);
                GUI.lbMoves.Items.Add($"{move.PieceMoved} {move.From} To {move.To}");

                LiveBoard.NextTurn();
                if (LiveBoard.WhosTurn.isChecked)
                {
                    if (LiveBoard.CheckMate())
                        EndGame();
                    else
                        GUI.RenameHeader($"Check! Go {LiveBoard.WhosTurn}");

                }
                else
                    GUI.RenameHeader($"Go {LiveBoard.WhosTurn}");
            }
        }

        public void ArchiveMove(ChessMove newMove)
        {
            if (Moves.Count > Moves_Index + 1)
                Moves = Moves.GetRange(0, Moves_Index + 1); //Clear future moves

            Moves_Index++;
            Moves.Add(newMove);
        }

        public bool UndoMove()
        {
            if (Moves_Index == -1)
                return false; //Cant undo when theres no moves

            ChessMove move = Moves[Moves_Index];
            LiveBoard.UndoMove_GamePiece(move);

            Moves_Index--;

            LiveBoard.NextTurn();

            return true;
        }

        public bool RedoMove()
        {
            if (Moves.Count == Moves_Index + 1)
                return false; //Can't redo with no future moves

            Moves_Index++;

            ChessMove move = Moves[Moves_Index];
            LiveBoard.Move_GamePiece(move);

            LiveBoard.NextTurn();

            return true;
        }

        public void EndGame()
        {
            Player winner = LiveBoard.playerOne.isChecked ? LiveBoard.playerTwo : LiveBoard.playerOne;
            Player loser = LiveBoard.playerOne.isChecked? LiveBoard.playerOne : LiveBoard.playerTwo;

            GUI.RenameHeader($"CheckMate! {winner} Wins!");
        }
    }
}


