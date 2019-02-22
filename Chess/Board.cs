using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        // Sets status of Cells for possible moves for selected Gamepiece
        //
        // NOTE requires a catch for pawns to prevent diagonal neutral movents 
        public void CanMove(Cell activeCell)
        {
            //Get blind move instructions for specific Gamepiece
            List<GamePiece.BlindMove> blindMoves = activeCell.Piece.BlindMoves();

            foreach(GamePiece.BlindMove bMove in blindMoves)
            {
                int moves = 0;
                Point testPoint = AddPoints(activeCell.ID, bMove.Direction);

                // Loop while in Board's bounds AND path is permited
                while (bMove.Limit != moves && testPoint.X <= 7 && testPoint.X >= 0 && testPoint.Y <= 7 && testPoint.Y >= 0)
                {
                    // See if path is clear & Set test Cell's status
                    if (!InvestigateBlind(testPoint, bMove.Condition)) break;

                    testPoint = AddPoints(testPoint, bMove.Direction);
                    moves++;
                }
            }
        }

        // Investigates if a blind move is valid
        // Returns: Bool if more moves in that direction are possible
        // Sets: investigated Cells status
        private bool InvestigateBlind(Point target, Cell.State condition)
        {
            Cell targetCell = Cells[PointIndex(target)];
            bool continueDir = false;

            // cell is empty
            if (targetCell.Piece is null)
            {
                // Good to go
                if (condition == Cell.State.Default || condition == Cell.State.Neutral)
                {
                    // Set as possible NEUTRAL
                    targetCell.Status = Cell.State.Neutral;
                    continueDir = true;
                }
                // GamePiece Condition doesn't permit it
                else
                {
                    // Deny
                    continueDir = false;
                }
            }
            // cell is Enemy
            else if (targetCell.Piece.TeamColor != WhosTurn.TeamColor)
            {
                // Good to go
                if (condition == Cell.State.Default || condition == Cell.State.Enemy)
                {
                    // set as possible ATTACK
                    targetCell.Status = Cell.State.Enemy;
                    continueDir = false;
                }
                // GamePiece Condition doesn't permit it
                else
                {
                    // Deny
                    continueDir = false;
                }
                // set as INVALID
                return false;
            }
            // cell is currently owned by same player
            else
            {
                // Deny
                continueDir = false;
            }

            return continueDir;
        }

        public void HighlightCells()
        {
            foreach(Cell c in Cells) c.CellColor();
        }

        public void ClearCellsStatus()
        {
            foreach (Cell c in Cells)
            {
                c.Status = Cell.State.Default;
                c.CellColor();

                if (!(c.Piece is null))
                    c.UIButton.Content = c.Piece.Img;
                else
                    c.UIButton.Content = null;
            }
        }

        public int InRange(int value)
        {
            int max = 7;
            int min = 0;
            int result = Math.Max(Math.Min(value, max), min);
            return result;
        }

        private Point AddPoints(Point p1, Point p2)
        {
            p1.X += p2.X;
            p1.Y += p2.Y;

            return p1;
        }

        // Allows finding cell of specific point in Board.Cells List
        private int PointIndex(Point p1)
        {
            return p1.X + (p1.Y * 8);
        }
    }
}
