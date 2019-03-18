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
    public class Board
    {
        public List<Cell> Cells = new List<Cell>();
        public List<ChessMove> Moves = new List<ChessMove>();
        private Controller controller;
        
        public Player playerOne { get { return controller.playerOne; } } // Copy Cats
        public Player playerTwo { get { return controller.playerTwo; } } // Copy Cats
        public Player WhosTurn { get{ return controller.WhosTurn; } } // Copy Cats

        //public delegate void DelCell(Cell c);
        //public DelCell delButtons = null;

        public Board(Controller contr)
        {
            controller = contr;

            GenerateBoard();
        }

        private void GenerateBoard()
        {
            if (Cells.Count == 64) return; // Quick fix to prevent Bot from using this function

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell cTemp = new Cell(x, y);

                    // Add gamepieces to reference list
                    if (!(cTemp.Piece is null))
                    {
                        if (cTemp.Piece.TeamColor == Color.White)
                            playerOne.MyPieces.Add(cTemp.Piece);
                        else
                            playerTwo.MyPieces.Add(cTemp.Piece);
                    }
                    // Delegate the Cell to the UI of MainWindow
                    //delButtons?.Invoke(cTemp);
                    cTemp.UIButton.Click += controller.GUI.Cell_Click;
                    controller.GUI.MyMainPanel.Children.Add(cTemp.UIButton);
                    Cells.Add(cTemp);
                }
            }
        }

        public void ClearBoard()
        {
           for(int i = 0; i < Cells.Count; i++)
            {
                controller.GUI.MyMainPanel.Children.Remove(Cells[i].UIButton);
            }
        }

        /// <summary>
        /// Returns list of All legal ChessMoves for a GamePiece
        /// </summary>
        /// <param name="piece">GamePiece that is being investigated</param>
        public List<ChessMove> PossibleMoves(GamePiece piece)
        {
            if (piece is null)
                return null; // illegal - Empty space
            if (piece.TeamColor != WhosTurn.TeamColor)
                return null; // illegal - Other players piece

            Cell fromCell = Cells.GetCell(piece.Location);

            List<ChessMove> possibleMoves = new List<ChessMove>(); // Returning List of Possible Moves
            List<BlindMove> blindMoves = fromCell.Piece.BlindMoves(); // Blind move instructions for Gamepiece

            GamePiece king = WhosTurn.MyPieces.Find(gp => gp is King);

            foreach (BlindMove bMove in blindMoves)
            {                
                Cell focusCell = Cells.NextCell(fromCell, bMove.Direction); // The Starting Point
                int moveCount = 0;
                Condition moveType = Condition.Neutral;

                // Increment through Instructions
                while (!(focusCell is null) && bMove.Limit != moveCount && moveType == Condition.Neutral)
                {
                    moveType = MovePossibility(fromCell.Piece, fromCell, focusCell, bMove.Condition);

                    // ADD to List of Possible ChessMoves
                    if (moveType == Condition.Neutral || moveType == Condition.Attack || moveType == Condition.enPassant || moveType == Condition.Castling)
                    {
                        ChessMove move;
                        if (moveType == Condition.enPassant) // *Special Move - Pawn captures Pawn via Enpassant
                        {
                            GamePiece captured = Cells.Find(c => !(c.enPassantPawn is null)).enPassantPawn;
                            move = new ChessMove(fromCell, focusCell, fromCell.Piece, captured, moveType, Cells.GetCell(captured.Location));
                        }
                        else if(moveType == Condition.Castling) // *Special Move - Castling, insert Rook into ChessMove
                        {
                            Rook rook = (Rook)Cells.GetCell((Point)bMove.OtherInfo).Piece;
                            int xDirection = bMove.Direction.X / 2;
                            Cell rookFrom = Cells.GetCell(rook.Location);
                            Cell rookTo = Cells.GetCell(new Point(fromCell.ID.X + xDirection, fromCell.ID.Y));

                            ChessMove rookMove = new ChessMove(rookFrom, rookTo, rook, null, Condition.Castling);

                            move = new ChessMove(fromCell, focusCell, fromCell.Piece, null, moveType, rookMove);
                        }
                        else // Regular Move
                            move = new ChessMove(fromCell, focusCell, fromCell.Piece, focusCell.Piece, moveType);

                        //Look in the future
                        move = MovePiece(move);
                        bool isKingSafe = IsSafe(king.Location, WhosTurn);
                        move = UndoMovePiece(move);

                        if (isKingSafe)
                            possibleMoves.Add(move);
                    }

                    focusCell = Cells.NextCell(focusCell, bMove.Direction);
                    moveCount++;
                }
            }

            return possibleMoves;
        }

        // Investigates if a move is Legal
        private Condition MovePossibility(GamePiece piece, Cell fromCell, Cell toCell, Condition condition)
        {
            Condition result = Condition.Default;

            if (fromCell == toCell)
                return Condition.Active; // Already there

            // cell is Empty
            if (toCell.Piece is null) // && condition != Condition.Attack)
            {
                // Legal Neutral
                if (condition == Condition.Default || condition == Condition.Neutral)
                {
                    result = Condition.Neutral;
                }
                // Legal Pawn Enpassant Move
                else if(piece is Pawn && condition == Condition.Attack && !(toCell.enPassantPawn is null))
                {
                    result = Condition.enPassant;
                }
            }
            // cell is Enemy
            else if (toCell.Piece != null && toCell.Piece.TeamColor != WhosTurn.TeamColor) // && condition != Condition.Neutral)
            {
                // Legal Attack
                if (condition == Condition.Default || condition == Condition.Attack)
                {
                    result = Condition.Attack;
                }
            }

            if(condition == Condition.Castling)
            {
                Player player = piece.TeamColor == playerOne.TeamColor ? playerOne : playerTwo;
                // get Rook
                int xDirection = fromCell.ID.X > toCell.ID.X ? -1 : 1;
                Point rookLocation = xDirection == 1 ? new Point(7, fromCell.ID.Y) : new Point(0, fromCell.ID.Y);
                Rook rook = (Rook)player.MyPieces.Find(gp => gp.ID == rookLocation);
                
                if (rook is null)
                    result = Condition.Illegal; // Rook is missing
                else
                {
                    if (piece.moveCount != 0 && rook.moveCount != 0)
                        result = Condition.Illegal; // Cannot Castle when King & Rook are not in original locations
                    else
                    {
                        Cell focus = Cells.NextCell(fromCell, new Point(xDirection, 0));
                        while (focus.Piece is null) // find next gamepiece in the movement direction
                            focus = Cells.NextCell(focus, new Point(xDirection, 0));

                        if (!ReferenceEquals(rook, focus.Piece))
                            result = Condition.Illegal; // Other Pieces in the way
                        else
                        {
                            if (player.isChecked == true)
                                result = Condition.Illegal; // Cannot Castle when in Check
                            else
                            {
                                Point passThrough = new Point(fromCell.ID.X + xDirection, fromCell.ID.Y);
                                if (!IsSafe(passThrough, WhosTurn))
                                    result = Condition.Illegal; // Cannot Castle through Check
                                else
                                    result = Condition.Castling; // Castling Permitted! (Well still one more test in PossibleMoves())
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Checks if location is safe from attack
        /// </summary>
        /// <param name="location">The questioned location (uses the current live Board)</param>
        /// <param name="whosAsking">The opponent of 'whosAsking' will be checked for attack possibilties</param>
        /// <returns></returns>
        public bool IsSafe(Point location, Player whosAsking)
        {
            Cell safeCell = Cells.GetCell(location);
            // Find Opponent
            Player Opponent = ReferenceEquals(whosAsking, playerOne) ? playerTwo : playerOne;

            // Check possible moves for Opponent's Live pieces
            foreach (GamePiece piece in Opponent.MyPieces.FindAll(p => p.isAlive))
            {
                List<BlindMove> blindMoves = piece.BlindMoves(); // GamePieces Blind Moves
                Cell fromCell = Cells.GetCell(piece.Location); // GamePiece's Cell

                // Iterate through those moves
                foreach (BlindMove bMove in blindMoves)
                {
                    // Only look at Attack permitted moves (No pawn forward moves)
                    if (bMove.Condition != Condition.Neutral)
                    {
                        Cell focusCell = fromCell;
                        int moveCount = 0;

                        // Increment through Instructions
                        do
                        {
                            // Increment direction
                            focusCell = Cells.NextCell(focusCell, bMove.Direction);
                            
                            // FOUND A MATCH TO CELL
                            if (ReferenceEquals(safeCell, focusCell))
                                return false; // Only need one attacker

                            moveCount++;
                        }
                        while (!(focusCell is null) && bMove.Limit != moveCount && focusCell.Piece is null) ;
                    }

                }
            }

            return true;
        }

        // Determines if theres a CHECKMATE
        public bool isCheckMate()
        {
            foreach(GamePiece piece in WhosTurn.MyPieces.FindAll(gp => gp.isAlive))
            {
                if (PossibleMoves(piece).Count > 0)
                    return false;
            }

            return true;
        }

        public void HighlightBoard(List<ChessMove> moves = null)
        {
            if(moves is null || moves.Count == 0)
                Cells.ForEach(c => c.ChangeState(Condition.Default));
            else
            {
                moves[0].From.ChangeState(Condition.Active);

                foreach(ChessMove mve in moves)
                {
                    mve.To.ChangeState(mve.MoveType);
                }
            }
        }

        public ChessMove MovePiece(ChessMove move)
        {
            if (move.MoveType == Condition.Attack)
            {
                // Capture Enemy GamePiece
                move.PieceCaptured.isAlive = false;
                move.PieceCaptured.Location = Point.Empty;
            }
            else if(move.MoveType == Condition.enPassant)
            {
                // EnPassant! Pawn captures pawn via Special move

                // Capture Enemy GamePiece
                move.PieceCaptured.isAlive = false;
                move.PieceCaptured.Location = Point.Empty;

                move.To.enPassantPawn = null;

                // Cast OtherInfo as Cell (It better be)
                if (!(move.OtherInfo is Cell actualPawnLocation)) 
                    throw new ArgumentException("ChessMove.OtherInfo was supposed to contain Cell object for Enpassant move");

                actualPawnLocation.Piece = null;
            }
            else if (move.MoveType == Condition.Castling)
            {
                if (!(move.OtherInfo is ChessMove rookMove))
                    throw new ArgumentException("ChessMove.OtherInfo was missing RookMove for Castling");

                rookMove.To.Piece = rookMove.PieceMoved;
                rookMove.PieceMoved.Location = rookMove.To.ID;

                rookMove.From.Piece = null;
            }

            // Move Active GamePiece
            move.To.Piece =  move.PieceMoved;
            move.PieceMoved.Location = move.To.ID;
            move.From.Piece = null;
            move.PieceMoved.moveCount++;

            // Check if Pawn and went 2 steps - (Enpassant) Flag if so
            GamePiece.ChecknFlagEnpassant(Cells, move);

            // Check if Pawn is at the End Zone! QUEEEN!!
            move = move.PieceMoved.Pawn2Queen(move, WhosTurn);

            return move;
        }       
        
        public ChessMove UndoMovePiece(ChessMove move)
        {
            if (move.MoveType == Condition.Attack)
            {
                // UnCapture Enemy GamePiece
                move.PieceCaptured.isAlive = true;
                move.PieceCaptured.Location = move.To.ID;
                move.To.Piece = move.PieceCaptured;
            }
            else if (move.MoveType == Condition.enPassant)
            {
                // Cast OtherInfo as Cell (It better be)
                if (!(move.OtherInfo is Cell actualPawnLocation))
                    throw new ArgumentException("ChessMove.OtherInfo was supposed to contain Cell object for Enpassant move");

                // Undo EnPassant! Pawn captures pawn via Special move

                move.PieceCaptured.isAlive = true;
                move.PieceCaptured.Location = actualPawnLocation.ID;

                move.To.enPassantPawn = move.PieceCaptured;
                move.To.Piece = null;
                actualPawnLocation.Piece = move.PieceCaptured;
            }
            else if (move.MoveType == Condition.Castling)
            {
                if (!(move.OtherInfo is ChessMove rookMove))
                    throw new ArgumentException("ChessMove.OtherInfo was missing RookMove for Castling");

                rookMove.From.Piece = rookMove.PieceMoved;
                rookMove.PieceMoved.Location = rookMove.From.ID;

                rookMove.To.Piece = null;
                move.To.Piece = null;
            }
            else
                move.To.Piece = null;

            // Move Active GamePiece
            move.From.Piece = move.PieceMoved;
            move.PieceMoved.Location = move.From.ID;
            move.PieceMoved.moveCount--;

            //Check if Pawn and went 2 steps - (Enpassant) UnFlag if so
            GamePiece.ChecknFlagEnpassant(Cells, move, true);

            // Check if Pawn was at the End Zone! UNDO QUEEEN!!
            move = move.PieceMoved.Pawn2Queen(move, WhosTurn, true);

            return move;
        }

        // Clear any enPassant - the option to attack expires after one turn
        public void ClearEnpassant()
        {
            // find if any
            Cell passedCell = Cells.Find(c => !(c.enPassantPawn is null) && c.enPassantPawn.TeamColor == WhosTurn.TeamColor);

            if (!(passedCell is null))
                passedCell.enPassantPawn = null;
        }
    }
}
