using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessTools;

namespace Chess
{
    public class Board
    {
        public List<Cell> Cells = new List<Cell>();

        public Cell ActiveCell;
        public List<GamePiece> BlackDead = new List<GamePiece>();
        public List<GamePiece> WhiteDead = new List<GamePiece>();
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;

        public delegate void DelCell(Cell c);
        public DelCell delButtons = null;

        public Board()
        {
            // Future - See if you can insert GenerateBoard() in here
        }

        public void GenerateBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell cTemp = new Cell(x, y);
                    // Delegate the Cell to the UI of MainWindow
                    delButtons?.Invoke(cTemp);
                    Cells.Add(cTemp);
                }
            }

            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = playerOne;
        }

        // Toggles active player after a move
        public void NextTurn()
        {
            if (WhosTurn == playerOne)
                WhosTurn = playerTwo;
            else
                WhosTurn = playerOne;
        }

        // Checks if selection was legal
        public bool SelectCell(Cell focusCell)
        {
            // Illegal - Empty space
            if (focusCell.Piece is null)
                return false;
            // Illegal - Other players piece
            if (focusCell.Piece.TeamColor != WhosTurn.TeamColor)
                return false;

            // Everything Checks out
            return true;
        }

        /// <summary>
        /// Sets status of Cells for possible moves for selected Gamepiece
        /// </summary>
        /// <param name="activeCell">Cell that owns GamePiece thats being moved</param>
        public List<CanMove> PossibleMoves(Cell activeCell)
        {
            //Get blind move instructions for specific Gamepiece
            List<BlindMove> blindMoves = activeCell.Piece.BlindMoves();
            List<CanMove> possibleMoves = new List<CanMove>();

            foreach(BlindMove bMove in blindMoves)
            {
                int moves = 0;
                Point testPoint = AddPoints(activeCell.ID, bMove.Direction);

                // Loop while in Board's bounds AND path is permited
                while (bMove.Limit != moves && InBoardsRange(testPoint))
                {
                    Cell targetCell = Cells[PointIndex(testPoint)];
                    int moveType = MovePossibility(targetCell, bMove.Condition);

                    // only neutral and attack moves permitted
                    if (moveType > 0)
                        possibleMoves.Add(new CanMove(testPoint, targetCell.Status));

                    // Break if the moveType isnt neutral
                    if (moveType != 1) break;

                    testPoint = AddPoints(testPoint, bMove.Direction);
                    moves++;
                }
            }

            return possibleMoves;
        }

        private int MovePossibility(Cell targetCell, CellState condition)
        {
            int result = 0;

            // cell is empty
            if (targetCell.Piece is null)
            {
                // Good to go
                if (condition == CellState.Default || condition == CellState.Neutral)
                {
                    // Set as possible NEUTRAL
                    targetCell.Status = CellState.Neutral;
                    result = 1;
                }
                // GamePiece Condition doesn't permit it
                else
                {
                    // Deny
                    result = 0;
                }
            }
            // cell is Enemy
            else if (targetCell.Piece.TeamColor != WhosTurn.TeamColor)
            {
                // Good to go
                if (condition == CellState.Default || condition == CellState.Enemy)
                {
                    // set as possible ATTACK
                    targetCell.Status = CellState.Enemy;
                    result = 2;
                }
                // GamePiece Condition doesn't permit it
                else
                {
                    // Deny
                    result = 0;
                }
            }

            return result;
        }

        public void HighlightCells()
        {
            foreach(Cell c in Cells) c.CellColor();
        }

        public void ClearCellsStatus()
        {
            foreach (Cell c in Cells)
            {
                c.Status = CellState.Default;
                c.CellColor();

                if (!(c.Piece is null))
                    c.UIButton.Content = c.Piece.Img;
                else
                    c.UIButton.Content = null;
            }
        }

        /// <summary>
        /// Tests to see if Point is within Boards scope
        /// </summary>
        /// <param name="test"></param>
        /// <returns>true if it is</returns>
        public bool InBoardsRange(Point test)
        {
            int max = 7;
            int min = 0;

            return  test.X <= max && test.X >= min && 
                    test.Y <= max && test.Y >= min;
        }

        private Point AddPoints(Point p1, Point p2)
        {
            p1.X += p2.X;
            p1.Y += p2.Y;

            return p1;
        }

        /// <summary>
        /// Allows finding cell of specific point in Board.Cells List
        /// </summary>
        /// <param name="cell">X & Y of cell on board</param>
        /// <returns>Index of the cell in List of Cells for the Board</returns>
        private int PointIndex(Point cell)
        {
            return cell.X + (cell.Y * 8);
        }
    }
}
