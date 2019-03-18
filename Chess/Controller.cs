using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
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

        public List<ChessMove> MoveArchive { get; private set; } = new List<ChessMove>();
        private int Moves_Index = -1; // Points to the Move currently viewed on LiveBoard 
        public List<ChessMove> tempPossMoves;
        public bool checkMate = false;
        public volatile bool gameOn = false;
        public bool GameOnView {
            set {
                if (!value)
                {
                    gameOn = false;
                    GUI._GameOnDisplay.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    gameOn = true;
                    GUI._GameOnDisplay.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public volatile int speed = 100;
        public volatile bool animate = true;
        private Thread _BotMatchThread = null;


        public MainWindow GUI;

        public Controller(MainWindow gui)
        {
            GUI = gui;
        }

        public bool NewGame(string brain1, string brain2)
        {
            if (gameOn) //!(_BotMatchThread is null)) // Check if current Bot Match in progress
            {
                GameOnView = false; // Flag Bot Match to End
                GUI._btnNewGame.Content = "New Game";
                return false;
            }
            
            GUI._btnNewGame.Content = "End Game";
            GameOnView = true;
            checkMate = false;

            if (!(LiveBoard is null))
            {
                LiveBoard.ClearBoard(); // Make sure to clear previous board
                ClearArchive();
            }

            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = playerOne;
            LiveBoard = new Board(this);

            if (!brain1.Equals("Human"))
                playerOne.BotThePlayer(brain1, this.LiveBoard);
            if (!brain2.Equals("Human"))
                playerTwo.BotThePlayer(brain2, this.LiveBoard);

            GUI.UpdateGameInfo(brain1, brain2); // 

            if (playerOne.isBot && playerTwo.isBot)
                BotBattleThreadStart();                 // Bot vs Bot (Entire Match Thread)
            else if (WhosTurn.isBot)
                BotThreadStart(WhosTurn.BotBrain); // Bots first move (1 Turn Thread)
            else
                return true;                            // Human Players first move

            return true;
        }

        public bool EndGame()
        {
            if (!(_BotMatchThread is null)) // Check if current Bot Match in progress
            {
                gameOn = false; // Flag Bot Match to End
                return false;
            }

            if (!(LiveBoard is null))
                LiveBoard.ClearBoard(); // Make sure to clear previous board

            return true;
        }
        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Human Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////
        public void BoardClick(Cell focusCell)
        {
            if (!gameOn)
                return;
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
                GUI.DispatchInvoke_UIUpdate(move);

                if(WhosTurn.isBot)
                    BotThreadStart(WhosTurn.BotBrain);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Bot Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////

        // Used for Initiating a Bot vs Bot Game
        private bool BotBattleThreadStart()
        {
            _BotMatchThread = new Thread(BotsBattle);
            _BotMatchThread.IsBackground = true;
            _BotMatchThread.Start();

            return true;
        }

        // Used for Initiating a Bot Turn in Human vs Bot Game
        private bool BotThreadStart(BotController bot)
        {
            Thread thread = new Thread(BotMove);
            thread.IsBackground = true;
            thread.Start(bot);

            return true;
        }

        public void BotsBattle(object obj = null)
        {
            if (WhosTurn.BotBrain is null)
                return;

            while(!checkMate && gameOn)
            {
                BotMove(WhosTurn.BotBrain);

                Console.WriteLine($"BotMove: {WhosTurn}");
            }

            _BotMatchThread = null;
            return;
        }

        private void BotMove(object obj)
        {
            BotController bot = (BotController)obj;

            if (gameOn && LiveBoard.WhosTurn == bot.Me && !checkMate)
            {
                Thread.Sleep(speed);
                ChessMove move = bot.MyTurn(); // Request Move from Bot


                if (move.From is null)
                    return; //Error with Bot

                if(animate)
                    AnimateMove(move); // Slow down what happened

                BoardMove(move);
                GUI.DispatchInvoke_UIUpdate(move);
            }
            else
                return; // CheckMate
        }

        private void AnimateMove(ChessMove move)
        {
            for (int i = 0; i < 2; i++)
            {
                GUI.DispatchInvoke_UIAnimate(move.From, Condition.Active);
                Thread.Sleep(speed);
                GUI.DispatchInvoke_UIAnimate(move.From, Condition.Default);
                Thread.Sleep(speed/2);
            }

            for (int i = 0; i < 2; i++)
            {
                GUI.DispatchInvoke_UIAnimate(move.To, move.MoveType);
                Thread.Sleep(speed);
                GUI.DispatchInvoke_UIAnimate(move.To, Condition.Default);
                Thread.Sleep(speed/2);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                 Tools & Helper Functions
        //
        /////////////////////////////////////////////////////////////////////////////////////
        
        // Implements Move, Archives Move, Activates NextTurn(), and Updates UI
        private bool BoardMove(ChessMove move)
        {
            move = LiveBoard.MovePiece(move);
            ArchiveMove(move);
            
            NextTurn();
            
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
            MoveArchive[Moves_Index] = LiveBoard.UndoMovePiece(move);

            Moves_Index--;

            NextTurn();

            if (WhosTurn.isBot) // Skip to Human Players former move
                UndoMove();

            GUI.DispatchInvoke_UIUpdate(move);
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
            MoveArchive[Moves_Index] = LiveBoard.MovePiece(move);

            NextTurn();

            if (WhosTurn.isBot) // Skip to Human Players latter move
                RedoMove();

            GUI.DispatchInvoke_UIUpdate(move);
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
            GUI.lbMoves.Items.Clear();
        }

        //==============================================================================
        //                                  The End
        //==============================================================================
        public void CheckMate()
        {
            Player winner = LiveBoard.playerOne.isChecked ? LiveBoard.playerTwo : LiveBoard.playerOne;
            Player loser = LiveBoard.playerOne.isChecked? LiveBoard.playerOne : LiveBoard.playerTwo;

            checkMate = true;
            GameOnView = false;

            GUI._btnNewGame.Content = "New Game";
            GUI.RenameHeader($"CheckMate! {winner} Wins! {MoveArchive.Count/2} moves");
            GUI._lbStats.Items.Add($"{winner} Wins!");
        }

        //public void GenerateGame()
        //{
        //    LiveBoard.GenerateBoard();
        //}
    }
}


