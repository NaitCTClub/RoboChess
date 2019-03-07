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

        public List<Cell> Cells = new List<Cell>();
        public List<Move> Moves = new List<Move>();

        public Cell ActiveCell; // Used for Human Player
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
            // Clear all previous cell Statuses
            Cells.ForEach(c => c.ChangeState(CellState.Default));

            List<CanMove> possibleMoves = new List<CanMove>();

            // illegal - Empty space
            if (focusCell.Piece is null)
                return possibleMoves;
            // illegal - Other players piece
            if (focusCell.Piece.TeamColor != WhosTurn.TeamColor)
                return possibleMoves;

            //Get blind move instructions for specific Gamepiece
            List<BlindMove> blindMoves = focusCell.Piece.BlindMoves();

            foreach(BlindMove bMove in blindMoves)
            {
                int moveCount = 0;
                Point testPoint = AddPoints(focusCell.ID, bMove.Direction);

                // Loop while in Board's bounds AND path is permited
                while (bMove.Limit != moveCount && InBoardsRange(testPoint))
                {
                    // Grab Cell associated to Point
                    Cell targetCell = Cells[PointIndex(testPoint)];

                    MoveType moveType = MovePossibility(focusCell, targetCell, bMove.Condition);

                    // Highlight Cell
                    if (moveType == MoveType.Neutral)
                        targetCell.ChangeState(CellState.Neutral);
                    else if (moveType == MoveType.Attack)
                        targetCell.ChangeState(CellState.Enemy);

                    // only neutral and attack moves permitted
                    if (moveType != MoveType.Illegal)
                        possibleMoves.Add(new CanMove(testPoint, moveType));

                    // No more moves possible if somethings in the way
                    if (moveType != MoveType.Neutral) break;

                    testPoint = AddPoints(testPoint, bMove.Direction);
                    moveCount++;
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Investigates if a move is Legal
        /// </summary>
        /// <param name="focusCell"></param>
        /// <param name="targetCell"></param>
        /// <param name="condition"></param>
        /// <returns> illegal = 0, legal neutral = 1, legal attack = 2</returns>
        private MoveType MovePossibility(Cell focusCell, Cell targetCell, CellState condition, bool kingSafety = true)
        {
            MoveType result = MoveType.Illegal;

            // Special Case - KING
            if(focusCell.Piece is King && kingSafety)
            {
                // Move not permitted if Opponent can attack king next turn
                if (!IsSafe(targetCell, false)) return result;
            }

            // cell is empty
            if (targetCell.Piece is null && condition != CellState.Enemy)
            {
                // Legal Neutral
                if (condition == CellState.Default || condition == CellState.Neutral)
                {
                    // Set as possible NEUTRAL
                    result = MoveType.Neutral;
                }
            }
            // cell is Enemy
            else if (targetCell.Piece != null && targetCell.Piece.TeamColor != WhosTurn.TeamColor && condition != CellState.Neutral)
            {
                // Legal Attack
                if (condition == CellState.Default || condition == CellState.Enemy)
                {
                    // set as possible ATTACK
                    result = MoveType.Attack;
                }
            }

            return result;
        }

        // Only Filters moves blocked by other pieces in the way
        private List<CanMove> FilterBlindMove(Cell focusCell, BlindMove bMove)
        {
            List<CanMove> result = new List<CanMove>();

            int moveCount = 0;
            Point testPoint = AddPoints(focusCell.ID, bMove.Direction);

            // Loop while in Board's bounds AND path is permited
            while (bMove.Limit != moveCount && InBoardsRange(testPoint))
            {
                // Grab Cell associated to Point
                Cell targetCell = Cells[PointIndex(testPoint)];

                MoveType moveType = MoveType.Default;

                if (targetCell.Piece.TeamColor == WhosTurn.TeamColor ||
                   (targetCell.Piece.TeamColor != WhosTurn.TeamColor && bMove.Condition == CellState.Neutral))
                    moveType = MoveType.Illegal;
                  

                if (moveType != MoveType.Illegal)
                    result.Add(new CanMove(testPoint, moveType));

                // Break if the moveType isnt neutral
                if (!(targetCell.Piece is null)) break;

                testPoint = AddPoints(testPoint, bMove.Direction);
                moveCount++;
            }

            return result;
        }

        // Checks if Opponent can attack a cell on next turn
        private bool IsSafe(Cell cell, bool oppKingSafety = true)
        {
            // Find Opponent
            Player Opponent = ReferenceEquals(WhosTurn, playerOne) ? playerTwo : playerOne;

            // Check possible moves for Opponent's Live pieces
            foreach (GamePiece piece in Opponent.Pieces.FindAll( p => p.isAlive))
            {
                //Get blind move instructions for specific Gamepiece
                List<BlindMove> blindMoves = piece.BlindMoves();

                // Iterate through those moves
                foreach (BlindMove bMove in blindMoves)
                {
                    // Only look at Attack permitted moves (No pawn forward moves)
                    if (bMove.Condition != CellState.Neutral)
                    {
                        Cell focusCell = Cells[PointIndex(piece.Location)];
                        int moveCount = 0;
                        int moveType = 0; // 0 = Neutral move

                        Point testPoint = AddPoints(piece.Location, bMove.Direction);

                        // Loop while in Board's bounds AND path is permited
                        while (bMove.Limit != moveCount && InBoardsRange(testPoint) && moveType == 0)
                        {
                            // See if can target cell in question
                            Cell targetCell = Cells[PointIndex(testPoint)];

                            // Find Move Type at the targetCell
                            moveType = MovePossibility(focusCell, targetCell, bMove.Condition, oppKingSafety);

                            if (ReferenceEquals(targetCell, cell) && moveType != 0)
                                return false; // Only takes one attack

                            testPoint = AddPoints(testPoint, bMove.Direction);
                            moveCount++;
                        }
                    }
                }
            }

            return true;
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
            if (to.Status == CellState.Enemy)
                to.Piece.isAlive = false;
            

            // Move Active GamePiece
            to.Piece = from.Piece;
            to.Piece.Location = to.ID;
            from.Piece = null;

            // Clear all previous cell Statuses
            Cells.ForEach(c => c.ChangeState(CellState.Default));
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
