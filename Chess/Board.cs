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
        public struct Move
        {
            public GamePiece Piece;
            public Point From;
            public Point To;
            public GamePiece PieceCaptured;
        }

        public List<Move> Moves = new List<Move>();
        public List<Cell> Cells = new List<Cell>();

        public Cell ActiveCell;
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
            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell cTemp = new Cell(x, y);

                    // Add gamepieces to reference list
                    if(!(cTemp.Piece is null))
                    {
                        if (cTemp.Piece.TeamColor == Color.White)
                            playerOne.Pieces.Add(cTemp.Piece);
                        else
                            playerTwo.Pieces.Add(cTemp.Piece);
                    }
                    // Delegate the Cell to the UI of MainWindow
                    delButtons?.Invoke(cTemp);
                    Cells.Add(cTemp);
                }
            }
            
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

        /// <summary>
        /// Sets status of Cells for possible moves for selected Gamepiece
        /// </summary>
        /// <param name="activeCell">Cell that owns GamePiece thats being moved</param>
        public List<CanMove> PossibleMoves(Cell focusCell)
        {
            // illegal - Empty space
            if (focusCell.Piece is null)
                return;
            // illegal - Other players piece
            if (focusCell.Piece.TeamColor != WhosTurn.TeamColor)
                return;

            //Get blind move instructions for specific Gamepiece
            List<BlindMove> blindMoves = focusCell.Piece.BlindMoves();
            List<CanMove> possibleMoves = new List<CanMove>();

            foreach(BlindMove bMove in blindMoves)
            {
                int moves = 0;
                Point testPoint = AddPoints(focusCell.ID, bMove.Direction);

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

            // Special Case - KING
            if(condition == Cell.State.UnChecked)
            {
                // Move not permitted if Opponent can attack king next turn
                if (IsChecked(targetCell)) return result;
            }

            // cell is empty
            if (targetCell.Piece is null && condition != Cell.State.Enemy)
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
            else if (targetCell.Piece != null && targetCell.Piece.TeamColor != WhosTurn.TeamColor && condition != Cell.State.Neutral)
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

        // Checks if Opponent can attack the cell on next turn
        private bool IsChecked(Cell cell)
        {
            bool isChecked = false;
            Player Opponent;

            if (ReferenceEquals(WhosTurn, playerOne))
                Opponent = playerTwo;
            else
                Opponent = playerOne;

            // Check possible moves for Opponent
            foreach (GamePiece piece in Opponent.Pieces)
            {
                //Get blind move instructions for specific Gamepiece
                List<GamePiece.BlindMove> blindMoves = piece.BlindMoves();

                foreach (GamePiece.BlindMove bMove in blindMoves)
                {
                    int moves = 0;
                    Point testPoint = AddPoints(piece.Location, bMove.Direction);

                    // Loop while in Board's bounds AND path is permited
                    while (bMove.Limit != moves && testPoint.X <= 7 && testPoint.X >= 0 && testPoint.Y <= 7 && testPoint.Y >= 0)
                    {
                        // See if can target cell in question
                        Cell targetCell = Cells[PointIndex(testPoint)];
                        bool continueDir = false;

                        // Set as possible attack with continued direction
                        if (targetCell.Piece is null && bMove.Condition != Cell.State.Neutral)
                        {
                            if (ReferenceEquals(targetCell, cell))
                                isChecked = true;

                            continueDir = true;
                        }
                        // direction is stopped
                        else if (targetCell.Piece != null)
                        {
                            if (ReferenceEquals(targetCell, cell))
                                isChecked = true;
                        }

                        if (isChecked) return isChecked;
                        if (!continueDir) break;

                        testPoint = AddPoints(testPoint, bMove.Direction);
                        moves++;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tests to see if Point is within Boards scope
        /// </summary>
        /// <param name="test"></param>
        /// <returns>True if its inside the Board</returns>
        public bool InBoardsRange(Point test)
        {
            int max = 7;
            int min = 0;

            return  test.X <= max && test.X >= min && 
                    test.Y <= max && test.Y >= min;
        }

        public Move GamePieceMove(Cell from, Cell to)
        {
            Move newMove = new Move
            {
                Piece = from.Piece,
                From = from.ID,
                To = to.ID,
                PieceCaptured = to.Piece
            };

            // Capture Enemy GamePiece
            if (to.Status == Cell.State.Enemy)
                to.Piece.isAlive = false;
            

            // Move Active GamePiece
            to.Piece = from.Piece;
            to.Piece.Location = to.ID;
            from.Piece = null;

            // Clear all previous cell Statuses
            Cells.ForEach(c => c.CellStatus(Cell.State.Default));
            ActiveCell = null;

            NextTurn();

            return newMove;
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
        /// <param name="cell">X/Y of cell on board</param>
        /// <returns>Index of the cell in List of Cells for the Board</returns>
        private int PointIndex(Point cell)
        {
            return cell.X + (cell.Y * 8);
        }
    }
}
