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
        public Controller controller;
        public GameResult Result { get; set; } = GameResult.InProgress;
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }
        public Player WhosTurn { get; private set; }
        public List<Cell> Cells = new List<Cell>();
        public List<GamePiece> AllPieces { get; private set; } = new List<GamePiece>();
        public List<GamePiece> PromotionPieces { get; private set; } = new List<GamePiece>();
        public List<ChessMove> MoveArchive { get; private set; } = new List<ChessMove>();
        public int Moves_Index = -1; // Points to the Move currently viewed on LiveBoard 

        public const int StaleMax = 50;
        public int StaleMoveCount = 0;

        public bool isVirtual { get; private set; } // Flags instances that are in Bots World


        public Board(Controller control) // Live Board
        {
            controller = control;

            PlayerOne = new Player(Color.White, "Player One");
            PlayerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = PlayerOne;
            GenerateCells_GamePieces();
            AllPieces = PlayerOne.MyPieces.Concat(PlayerTwo.MyPieces).ToList();
        }

        public Board(Board liveBoard) // Virtual Board
        {
            isVirtual = true;

            PlayerOne = new Player(liveBoard.PlayerOne);
            PlayerTwo = new Player(liveBoard.PlayerTwo);
            WhosTurn = liveBoard.WhosTurn == liveBoard.PlayerOne? PlayerOne : PlayerTwo;

            foreach (GamePiece gp in liveBoard.AllPieces)
                AllPieces.Add(GamePiece.VirtualPiece(gp));

            PlayerOne.MyPieces = AllPieces.FindAll(gp => gp.TeamColor == Color.White);
            PlayerTwo.MyPieces = AllPieces.FindAll(gp => gp.TeamColor == Color.Black);

            foreach (Cell c in liveBoard.Cells)
            {
                Cell virCell = new Cell(c);
                virCell.Piece = AllPieces.Find(gp => gp.Location == virCell.ID && gp.isAlive);
                if(!(c.enPassantPawn is null))
                    virCell.enPassantPawn = AllPieces.Find(gp => gp.ID == c.enPassantPawn.ID);
                Cells.Add(virCell);
            }
        }

        private void GenerateCells_GamePieces()
        {
            if (Cells.Count == 64) return;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell cTemp = new Cell(x, y);

                    // Add gamepieces to reference list
                    if (!(cTemp.Piece is null))
                    {
                        if (cTemp.Piece.TeamColor == Color.White)
                            PlayerOne.MyPieces.Add(cTemp.Piece);
                        else
                            PlayerTwo.MyPieces.Add(cTemp.Piece);
                    }
                    // Delegate the Cell to the UI of MainWindow
                    //delButtons?.Invoke(cTemp);
                    cTemp.UIButton.Click += controller.GUI.Cell_Click;
                    controller.GUI.MyMainPanel.Children.Add(cTemp.UIButton);

                    Cells.Add(cTemp);
                }
            }
            
            PromotionPieces = GeneratePromotionPieces();
        }

        public void RemoveCellsFromUI()
        {
           if (isVirtual)
                return;

           for(int i = 0; i < Cells.Count; i++)
            {
                controller.GUI.MyMainPanel.Children.Remove(Cells[i].UIButton);
            }
        }

        // Toggles active player after a move
        public void ToggleTurn()
        {
            WhosTurn.isChecked = false;

            if (WhosTurn == PlayerOne)
                WhosTurn = PlayerTwo;
            else
                WhosTurn = PlayerOne;

            ClearEnpassant(); // Enpassant option expires after one turn
        }

        /// <summary>
        /// Returns list of All legal ChessMoves for a GamePiece
        /// </summary>
        /// <param name="piece">GamePiece that is being investigated</param>
        public List<ChessMove> PossibleMoves(GamePiece piece)
        {
            if (piece is null)
                return null; // illegal - Empty space

            Player activePlayer = piece.TeamColor == PlayerOne.TeamColor ? PlayerOne : PlayerTwo;
            Cell fromCell = Cells.GetCell(piece.Location);
            List<ChessMove> possibleMoves = new List<ChessMove>(); // Returning List of Possible Moves
            GamePiece king = activePlayer.MyPieces.Find(gp => gp is King);

            //List<BlindMove> blindMoves = fromCell.Piece.BlindMoves(); // Blind move instructions for Gamepiece
            foreach (BlindMove bMove in fromCell.Piece.BlindMoves()) // Blind move instructions for Gamepiece
            {                
                Cell focusCell = Cells.NextCell(fromCell, bMove.Direction); // The Starting Point
                int moveCount = 0;
                Condition moveType = Condition.Neutral;

                // Increment through Instruction
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
                        move = MovePiece(move, true);
                        bool isKingSafe = IsSafe(king.Location, activePlayer);
                        move = UndoMovePiece(move, true);

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

            if (toCell.Piece is null) // cell is Empty
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
            else if (toCell.Piece != null && toCell.Piece.TeamColor != piece.TeamColor) // cell is Enemy
            {
                // Legal Attack
                if (condition == Condition.Default || condition == Condition.Attack)
                {
                    result = Condition.Attack;
                }
            }

            if(condition == Condition.Castling)
            {
                Player player = piece.TeamColor == PlayerOne.TeamColor ? PlayerOne : PlayerTwo;
                // get Rook
                int xDirection = fromCell.ID.X > toCell.ID.X ? -1 : 1;
                Point rookLocation = xDirection == 1 ? new Point(7, fromCell.ID.Y) : new Point(0, fromCell.ID.Y);
                Rook rook = (Rook)player.MyPieces.Find(gp => gp.ID == rookLocation && gp.moveCount == 0 && gp.isAlive);
                
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
            Player Opponent = ReferenceEquals(whosAsking, PlayerOne) ? PlayerTwo : PlayerOne;

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

        // Determines if theres a CHECKMATE against 'who'
        public bool CheckContinue(Player who)
        {
            // ** Future implementation of Draw (50 moves no Capture, 10 repeated moves)

            List<ChessMove> checkMoves = new List<ChessMove>();

            // Look up all possible moves for 'who'
            foreach (GamePiece piece in who.MyPieces.FindAll(gp => gp.isAlive))
            {
                checkMoves.AddRange(PossibleMoves(piece));
                if (checkMoves.Count > 0 && StaleMoveCount < StaleMax)
                    return true; // At least one possible move, No CheckMate, No Draw
            }

            if (StaleMoveCount == StaleMax)
                Result = GameResult.Draw; // Too many moves with no Pieces captured
            else if (IsSafe(who.MyPieces.Find(gp => gp is King).Location, who))
                Result = GameResult.StaleMate; // Not able to move but not Checked
            else
                Result = GameResult.CheckMate;

            return false;
        }

        public void HighlightBoard(List<ChessMove> moves = null)
        {
            if (isVirtual)
                return;

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

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                          Move Methods
        //
        /////////////////////////////////////////////////////////////////////////////////////
        

        //==============================================================================
        //                         Move GamePiece
        //==============================================================================
        // Implements Move, Archives Move, Activates ToggleTurn()
        public bool TakeTurn(ChessMove move)
        {
            move = MovePiece(move);
            ArchiveMove(move);
            ToggleTurn();

            if (!CheckContinue(WhosTurn)) // <======== **Check Promotion, Then CheckMate()
            {
                //Dispatcher.Invoke(new Action(delegate () { Fin(); }));
                return false;
            }


            return true;
        }

        //==============================================================================
        //                          Un-Move GamePiece
        //==============================================================================
        public bool UndoMove()
        {
            if (Moves_Index == -1)
                return false; //Cant undo when theres no moves

            ToggleTurn();

            ChessMove move = MoveArchive[Moves_Index];
            MoveArchive[Moves_Index] = UndoMovePiece(move);

            Moves_Index--;

            if (WhosTurn.isBot) // Skip to Human Players former move
                UndoMove();

            controller.GUI.DispatchInvoke_RemoveMove(move);
            return true;
        }

        //==============================================================================
        //                         Re-Move GamePiece
        //==============================================================================
        public bool RedoMove()
        {
            if (MoveArchive.Count == Moves_Index + 1)
                return false; //Can't redo with no future moves

            Moves_Index++;

            ChessMove move = MoveArchive[Moves_Index];
            MoveArchive[Moves_Index] = MovePiece(move);

            ToggleTurn();

            if (WhosTurn.isBot) // Skip to Human Players latter move
                RedoMove();

            controller.GUI.DispatchInvoke_AddMove(move);
            return true;
        }

        //==============================================================================
        //                   Archive Moves for stats & Undo/Redo
        //==============================================================================
        private void ArchiveMove(ChessMove newMove)
        {
            if (MoveArchive.Count > Moves_Index + 1)
                MoveArchive = MoveArchive.GetRange(0, Moves_Index + 1); //Clear future moves

            Moves_Index++;
            MoveArchive.Add(newMove);
        }

        private void ClearArchive()
        {
            Moves_Index = -1;
            MoveArchive.Clear();
            controller.GUI.lbMoves.Items.Clear();
        }

        //==============================================================================
        //                   The Actual Process Of Moving GamePiece
        //==============================================================================
        public ChessMove MovePiece(ChessMove move, bool foreShadow = false)
        {
            if (!foreShadow)
                StaleMoveCount = move.MoveType != Condition.Attack ? StaleMoveCount + 1 : 0;

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
            if(!foreShadow)
                move = move.PieceMoved.Promotion(move, WhosTurn, this);

            return move;
        }       
        
        public ChessMove UndoMovePiece(ChessMove move, bool foreShadow = false)
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
            if (!foreShadow)
                move.PieceMoved.UndoPromotion(move, WhosTurn, this);

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

        // Used for promoting Pawns from Bot Threads to prevent Breaks trying to create a GamePiece in off thread
        private List<GamePiece> GeneratePromotionPieces()
        {
            List<GamePiece> white_Blacks = new List<GamePiece>();
            
            for(int i = 0; i < 8; i++)
                white_Blacks.Add(new Queen(Color.White, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Knight(Color.White, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Bishop(Color.White, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Rook(Color.White, Point.Empty));

            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Queen(Color.Black, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Knight(Color.Black, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Bishop(Color.Black, Point.Empty));
            for (int i = 0; i < 8; i++)
                white_Blacks.Add(new Rook(Color.Black, Point.Empty));

            return white_Blacks;
        }
    }
}
