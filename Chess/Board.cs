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
                    if (!(cTemp.Piece is null))
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

            // Clear any enPassant
            Cell passedCell = Cells.Find(c => !(c.enPassantPawn is null) && c.enPassantPawn.TeamColor == WhosTurn.TeamColor);
            if(!(passedCell is null))
                passedCell.enPassantPawn = null;

            // See if King was Checked
            GamePiece king = WhosTurn.Pieces.Find(gp => gp is King);
            if (!IsSafe(king, WhosTurn))
                WhosTurn.isChecked = true;

            HighlightBoard(); // Clear cell statuses
        }

        /// <summary>
        /// Sets status of Cells for possible moves for selected Gamepiece
        /// </summary>
        /// <param name="activeCell">Cell that owns GamePiece thats being moved</param>
        public List<ChessMove> PossibleMoves(GamePiece piece)
        {
            if (piece is null)
                return null; // illegal - Empty space
            if (piece.TeamColor != WhosTurn.TeamColor)
                return null; // illegal - Other players piece

            Cell fromCell = Cells.GetCell(piece.Location);

            List<ChessMove> possibleMoves = new List<ChessMove>(); // Returning List of Possible Moves
            List<BlindMove> blindMoves = fromCell.Piece.BlindMoves(); // Blind move instructions for Gamepiece

            GamePiece king = WhosTurn.Pieces.Find(gp => gp is King);

            foreach (BlindMove bMove in blindMoves)
            {                
                Cell focusCell = Cells.NextCell(fromCell, bMove.Direction); // The Starting Point
                int moveCount = 0;
                Condition moveType = Condition.Neutral;

                // Increment through Instructions
                while (!(focusCell is null) && bMove.Limit != moveCount && moveType == Condition.Neutral)
                {
                    moveType = MovePossibility(fromCell, focusCell, bMove.Condition);

                    // ADD to List of Possible ChessMoves
                    if (moveType == Condition.Neutral || moveType == Condition.Attack || moveType == Condition.enPassant)
                    {
                        ChessMove move;
                        if (moveType == Condition.enPassant)
                        {
                            GamePiece captured = Cells.Find(c => !(c.enPassantPawn is null)).enPassantPawn;
                            move = new ChessMove(fromCell, focusCell, fromCell.Piece, captured, moveType, Cells.GetCell(captured.Location));
                        }
                        else
                            move = new ChessMove(fromCell, focusCell, fromCell.Piece, focusCell.Piece, moveType);

                        // Look in the future
                        this.Move_GamePiece(move);
                        bool isKingSafe = IsSafe(king, WhosTurn);
                        this.UndoMove_GamePiece(move);

                        if (isKingSafe)
                            possibleMoves.Add(move);
                    }

                    focusCell = Cells.NextCell(focusCell, bMove.Direction);
                    moveCount++;
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Investigates if a move is Legal
        /// </summary>
        /// <param name="fromCell"></param>
        /// <param name="toCell"></param>
        /// <param name="condition"></param>
        /// <returns> illegal = 0, legal neutral = 1, legal attack = 2</returns>
        private Condition MovePossibility(Cell fromCell, Cell toCell, Condition condition)
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
                else if(fromCell.Piece is Pawn && condition == Condition.Attack && !(toCell.enPassantPawn is null))
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

            return result;
        }

        // Checks if Opponent can attack a cell on next turn
        private bool IsSafe(GamePiece safePiece, Player whosAsking)
        {
            Cell safeCell = Cells.GetCell(safePiece.Location);
            // Find Opponent
            Player Opponent = ReferenceEquals(whosAsking, playerOne) ? playerTwo : playerOne;

            // Check possible moves for Opponent's Live pieces
            foreach (GamePiece piece in Opponent.Pieces.FindAll(p => p.isAlive))
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

        public bool CheckMate()
        {
            foreach(GamePiece piece in WhosTurn.Pieces.FindAll(gp => gp.isAlive))
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

        public ChessMove Move_GamePiece(ChessMove move)
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
                move.OtherCell.Piece = null;
            }

            // Move Active GamePiece
            move.To.Piece =  move.PieceMoved;
            move.PieceMoved.Location = move.To.ID;
            move.From.Piece = null;

            //Check if Pawn and went 2 steps - (Enpassant) Flag if so
            GamePiece.ChecknFlagEnpassant(Cells, move);

            return move;
        }       
        
        public ChessMove UndoMove_GamePiece(ChessMove move)
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
                // Undo EnPassant! Pawn captures pawn via Special move

                move.PieceCaptured.isAlive = true;
                move.PieceCaptured.Location = move.OtherCell.ID;

                move.To.enPassantPawn = move.PieceCaptured;
                move.To.Piece = null;
                move.OtherCell.Piece = move.PieceCaptured;
            }
            else
                move.To.Piece = null;

            // Move Active GamePiece
            move.From.Piece = move.PieceMoved;
            move.PieceMoved.Location = move.From.ID;

            //Check if Pawn and went 2 steps - (Enpassant) UnFlag if so
            GamePiece.ChecknFlagEnpassant(Cells, move, "undo");

            return move;
        }
    }
}
