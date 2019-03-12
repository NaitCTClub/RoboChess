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
        public Board LiveBoard { get; private set; }
        public Player playerOne { get; private set; }
        public Player playerTwo { get; private set; }
        public Player WhosTurn { get; private set; }
        public Bot BotBrain1 { get; private set; }
        public Bot BotBrain2 { get; private set; }

        public List<ChessMove> MoveArchive { get; private set; } = new List<ChessMove>();
        private int Moves_Index = -1; // Points to the Move currently viewed on LiveBoard 
        public List<ChessMove> tempPossMoves;
        public bool checkMate = false;


        public MainWindow GUI;

        public Controller(MainWindow gui)
        {
            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = playerOne;

            GUI = gui;
            LiveBoard = new Board(this);
            BotBrain2 = new Bot(LiveBoard, playerTwo);
            BotBrain1 = new Bot(LiveBoard, playerOne); // Initializing this automatically changes to bot vs bot interaction
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Human Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////
        public void BoardClick(Cell focusCell)
        {
            if (checkMate)
                return;
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
                tempPossMoves = LiveBoard.PossibleMoves(piece);

                LiveBoard.HighlightBoard(tempPossMoves);

                GUI.RenameHeader("Choose target Cell");
            }
            //==============================================================================
            //                          Move GamePiece
            //==============================================================================
            else
            {
                // MOVE,CAPTURE & TOGGLE TURN
                ChessMove move = tempPossMoves.Find(moveFind => moveFind.To == focusCell);

                BoardMove(move);

                BotMove(BotBrain2);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Bot Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////
        private bool BotMove(Bot bot)
        {
            if (LiveBoard.WhosTurn == bot.Me && !checkMate)
            {
                System.Threading.Thread.Sleep(100);
                ChessMove move = bot.MyTurn(); // Request Move from Bot

                GUI.AnimateMove(move); // Slow down what happened
                BoardMove(move);

                return true;
            }

            return false;
        }

        public void BotsBattle()
        {
            if (BotBrain1 is null)
                return;

            Bot botWhosTurn = BotBrain1;

            while(!checkMate)
            {
                BotMove(botWhosTurn);

                if (botWhosTurn == BotBrain1)
                    botWhosTurn = BotBrain2;
                else
                    botWhosTurn = BotBrain1;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                 Tools & Helper Functions
        //
        /////////////////////////////////////////////////////////////////////////////////////
        private bool BoardMove(ChessMove move)
        {
            LiveBoard.MovePiece(move);
            ArchiveMove(move);

            if(move.PieceCaptured is null)
                GUI.lbMoves.Items.Add($"{move.PieceMoved} {move.From} To {move.To}");
            else
                GUI.lbMoves.Items.Add($"{move.PieceMoved} {move.From} To {move.To} | Captured: {move.PieceCaptured}");

            NextTurn();
            if (WhosTurn.isChecked)
            {
                if (LiveBoard.CheckMate())
                    CheckMate();
                else
                    GUI.RenameHeader($"Check! Go {WhosTurn}");

            }
            else
                GUI.RenameHeader($"Go {WhosTurn}");

            return true;
        }

        // Toggles active player after a move
        private void NextTurn()
        {
            if (WhosTurn == playerOne)
                WhosTurn = playerTwo;
            else
                WhosTurn = playerOne;

            LiveBoard.ClearEnpassant(); // Enpassant option expires after one turn
            LiveBoard.HighlightBoard(); // Clear cell statuses

            WhosTurn.AmIChecked(LiveBoard); // Flag player if checked
        }

        //==============================================================================
        //                          Un-Move GamePiece
        //==============================================================================
        public bool UndoMove()
        {
            if (Moves_Index == -1)
                return false; //Cant undo when theres no moves

            ChessMove move = MoveArchive[Moves_Index];
            LiveBoard.UndoMovePiece(move);

            Moves_Index--;

            NextTurn();

            if (WhosTurn.isBot)
                UndoMove();

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
            LiveBoard.MovePiece(move);

            NextTurn();

            if (WhosTurn.isBot)
                RedoMove();

            return true;
        }

        //==============================================================================
        //                   Archive Moves for stats & Undo/Redo
        //==============================================================================
        public void ArchiveMove(ChessMove newMove)
        {
            if (MoveArchive.Count > Moves_Index + 1)
                MoveArchive = MoveArchive.GetRange(0, Moves_Index + 1); //Clear future moves

            Moves_Index++;
            MoveArchive.Add(newMove);
        }

        //==============================================================================
        //                                  The End
        //==============================================================================
        public void CheckMate()
        {
            Player winner = LiveBoard.playerOne.isChecked ? LiveBoard.playerTwo : LiveBoard.playerOne;
            Player loser = LiveBoard.playerOne.isChecked? LiveBoard.playerOne : LiveBoard.playerTwo;

            checkMate = true;

            GUI.RenameHeader($"CheckMate! {winner} Wins!");
        }

        public void GenerateGame()
        {
            LiveBoard.GenerateBoard();
        }
    }
}


