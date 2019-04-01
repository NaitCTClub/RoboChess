using System;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using ChessTools;
using static ChessTools.Library;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using Condition = ChessTools.Condition;

using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Chess
{
    public class Controller : Window
    {
        public struct nGame
        {
            public string bot1;
            public string bot2;
            public int gameCount;
        }
        public MainWindow GUI { get; private set; }

        public Board LiveBoard { get; private set; }
        public Player playerOne { get { return LiveBoard.PlayerOne; } }
        public Player playerTwo { get { return LiveBoard.PlayerTwo; } }
        public Player WhosTurn { get { return LiveBoard.WhosTurn; } }
        public List<ChessMove> TempMoves { get; private set; } = new List<ChessMove>(); // Used for Human Players
        public List<GameStat> GameStats { get; set; } = new List<GameStat>();
        public ObservableCollection<BotRecord> BotRecords { get; set; }

        public bool GameOnView {
            set {
                if (!value)
                {
                    gameOn = false;
                    if(!nGamesOn)
                        GUI.SetWindow_NoGameInProgress();
                }
                else
                {
                    gameOn = true;
                    if(nGamesPause)
                        GUI.SetWindow_PausedGameinProgress();
                    else
                        GUI.SetWindow_ActiveGameinProgress();
                    //GUI._GameOnDisplay.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public volatile int speed = 100;
        public volatile bool animate = true; // Flag to animate bot moves
        private Thread _BotMatchThread = null;
        private Thread _nMatchesThread = null;
        public volatile bool gameOn = false; // Flag of active Game
        public volatile bool nGamesOn = false; // Flag for active Multi Match Thread
        public volatile bool nGamesPause = false; // Flag to Pause Bot Thread
        public volatile bool RefreshBoard = true;
        public volatile bool ViewBotsEnd = true;

        public Controller(MainWindow gui)
        {
            BotRecords = BotController.GetBotRecords();
            GUI = gui;
        }

        public void PlayNGamesThreadStart(nGame game)
        {
            _nMatchesThread = new Thread(PlayNGames);
            _nMatchesThread.IsBackground = true;
            _nMatchesThread.Start(game);
        }

        private void PlayNGames(object obj)
        {
            if (nGamesOn)
                return; // Redundancy

            nGame game = (nGame)obj;
            nGamesOn = true;
            int i = 0;

            while (i < game.gameCount && nGamesOn)
            {
                i++;
                Dispatcher.Invoke(new Action( delegate () { NewGame(game.bot1, game.bot2); }));
                while (gameOn) ; // Wait for game to end
                if (ViewBotsEnd)
                    Thread.Sleep(2000);
                else
                    Thread.Sleep(10);
            }

            Dispatcher.Invoke(new Action(delegate () { GUI.SetWindow_NoGameInProgress(); }));
            nGamesOn = false;
            _nMatchesThread = null;
        }

        public void CancelGame()
        {
            LiveBoard.Result = GameResult.BoardFlipped;
            nGamesPause = false;
            nGamesOn = false;
            GameOnView = false;
        }

        public bool NewGame(string brain1, string brain2)
        {
            if (gameOn) // Check if current Match in progress **Isnt required anymore, I think..
            {
                // Stop current Match
                CancelGame();
                if(!(_BotMatchThread is null))
                    _BotMatchThread.Abort(); // BRUTE Force
                
                NewGame(brain1, brain2);
                return false;
            }
            if (brain1.Equals("Human") || brain2.Equals("Human"))
                GUI._HumanGameCommands.Visibility = Visibility.Visible;
            else
                GUI._HumanGameCommands.Visibility = Visibility.Collapsed;

            GameOnView = true;

            if (!(LiveBoard is null))
            {
                LiveBoard.RemoveCellsFromUI(); // Make sure to clear previous board
            }

            LiveBoard = new Board(this);

            if (!brain1.Equals("Human"))
                playerOne.BotThePlayer(brain1, LiveBoard); // Initiate Bot - give it a Virtual Board
            if (!brain2.Equals("Human"))
                playerTwo.BotThePlayer(brain2, LiveBoard); // Initiate Bot - give it a Virtual Board

            GUI.UpdateGameInfo(brain1, brain2); // 

            GUI.RenameHeader($"Go {WhosTurn}");

            // ======== Start the Game =================
            if (playerOne.isBot && playerTwo.isBot)
                BotBattleThreadStart();                 // Bot vs Bot (new Thread for entire Match)
            else if (WhosTurn.isBot)
                BotThreadStart(WhosTurn);               // Bots first move (new Thread for 1 turn)
            else
                return true;                            // Human Players first move

            return true;
        }

        public bool EndGame() // **Not being used
        {
            if (!(_BotMatchThread is null)) // Check if current Bot Match in progress
            {
                gameOn = false; // Flag Bot Match to End
                return false;
            }

            if (!(LiveBoard is null))
                LiveBoard.RemoveCellsFromUI(); // Make sure to clear previous board

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
            if (LiveBoard.Result != GameResult.InProgress) // ** See if redundant
                return;
            if (focusCell.Status == Condition.Active)
                return; // Clicking the currently selected cell does nothing

            //==============================================================================
            //                         Select GamePiece
            //==============================================================================
            if (focusCell.Status == Condition.Default)
            {
                LiveBoard.UpdateBoardGUI(); // ClearBoard highlights

                if (focusCell.Piece is null || focusCell.Piece.TeamColor != WhosTurn.TeamColor)
                    return;

                // Get & Set cell status for possible moves
                GamePiece piece = focusCell.Piece;
                TempMoves = LiveBoard.PossibleMoves(piece);

                LiveBoard.UpdateBoardGUI(TempMoves);

                GUI.RenameHeader("Choose target Cell");
            }
            //==============================================================================
            //                          Move GamePiece
            //==============================================================================
            else
            {
                // MOVE,CAPTURE & TOGGLE TURN
                ChessMove move = TempMoves.Find(moveFind => moveFind.To == focusCell);

                // MOVE GAME PIECE 
                LiveBoard.TakeTurn(move);
                GUI.UpdateWindow();

                if (LiveBoard.Result != GameResult.InProgress)
                    Fin(); // GAME complete

                if(WhosTurn.isBot)
                    BotThreadStart(WhosTurn);
            }
        }

        public bool UndoMove()
        {
            if (!gameOn)
                return false;

            return LiveBoard.UndoMove();
        }

        public bool RedoMove()
        {
            if (!gameOn)
                return false;

            return LiveBoard.RedoMove();
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
        private bool BotThreadStart(Player who)
        {
            Thread thread = new Thread(BotMove);
            thread.IsBackground = true;
            thread.Start(who);

            return true;
        }

        public void BotsBattle(object obj = null)
        {
            if (WhosTurn.BotBrain is null)
                return;

            while(LiveBoard.Result == GameResult.InProgress && gameOn)
            {
                if(nGamesPause)
                {
                    Dispatcher.Invoke(new Action(delegate () { GUI.UpdateWindow(); }));
                    while (nGamesPause) ;
                }

                BotMove(WhosTurn);

                //Console.WriteLine($"BotMove: {WhosTurn}");
            }

            Dispatcher.Invoke(new Action(delegate () { Fin(); })); // GAME complete
            _BotMatchThread = null;
            return;
        }

        private void BotMove(object obj)
        {
            if (!(obj is Player activePlayer))
                throw new ArgumentException("Botmove only accepts Player as parameter");

            if (!gameOn || LiveBoard.Result != GameResult.InProgress)
                return; // Should just be a safety net, This method shouldnt be called with these variables
            
            Thread.Sleep(speed);
            activePlayer.UpdateBot(LiveBoard);

            ChessMove move = activePlayer.BotBrain.MyTurn(); // Request Move from Bot

            if (move.From is null)
                return; //Error with Bot **ADD Catch (BoardFlipped by bot)

            move = VirtualtoLive(move); // Switch references to LiveBoard

            if (animate) // Animate Move
                AnimateMove(move);

            // MOVE GAME PIECE 
            LiveBoard.TakeTurn(move);

            if (RefreshBoard) // Update Window
                Dispatcher.Invoke(new Action(delegate () { GUI.UpdateWindow(); }));
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                 Tools & Helper Functions
        //
        /////////////////////////////////////////////////////////////////////////////////////

        // Return matching references in LiveBoard of 'move' in virtualBoard
        private ChessMove VirtualtoLive(ChessMove move)
        {
            ChessMove live = new ChessMove();
            live.From = LiveBoard.Cells.GetCell(move.From.ID);
            live.To = LiveBoard.Cells.GetCell(move.To.ID);
            live.PieceCaptured = LiveBoard.AllPieces.Find(piece => piece.Equals(move.PieceCaptured));
            live.PieceMoved = LiveBoard.AllPieces.Find(piece => piece.Equals(move.PieceMoved));
            live.MoveType = move.MoveType;
            if (move.MoveType == Condition.enPassant)
            {
                if (!(move.OtherInfo is Cell otherCell))
                    throw new ArgumentException($"Error with Enpassant ChessMove from Bot: {WhosTurn.BotBrain}");

                live.OtherInfo = LiveBoard.Cells.GetCell(otherCell.ID);
            }
            else if (move.MoveType == Condition.Castling && !(move.PieceMoved is Rook))
            {
                if (!(move.OtherInfo is ChessMove RookMove))
                    throw new ArgumentException($"Error with Castling ChessMove from Bot: {WhosTurn.BotBrain}");

                live.OtherInfo = VirtualtoLive(RookMove); // Recursion
            }

            return live;
        }

        private void AnimateMove(ChessMove move)
        {
            for (int i = 0; i < 2; i++)
            {
                GUI.DispatchInvoke_UIAnimate(move.From, Condition.Active);
                Thread.Sleep(speed);
                GUI.DispatchInvoke_UIAnimate(move.From, Condition.Default);
                Thread.Sleep(speed / 2);
            }

            for (int i = 0; i < 2; i++)
            {
                GUI.DispatchInvoke_UIAnimate(move.To, move.MoveType);
                Thread.Sleep(speed);
                GUI.DispatchInvoke_UIAnimate(move.To, Condition.Default);
                Thread.Sleep(speed / 2);
            }
        }

        //==============================================================================
        //                                  The End
        //==============================================================================
        public void Fin()
        {
            Player winner = LiveBoard.PlayerOne == WhosTurn ? LiveBoard.PlayerTwo : LiveBoard.PlayerOne;
            Player loser = LiveBoard.PlayerOne == WhosTurn? LiveBoard.PlayerOne : LiveBoard.PlayerTwo;
            string result;
            
            if (LiveBoard.Result == GameResult.BoardFlipped)
                result = $"{LiveBoard.Result}! We will never Know..";
            else if (LiveBoard.Result == GameResult.Draw)
                result = $"Draw. No end game here.";
            else
                result = $"{LiveBoard.Result}! {winner} Wins! {LiveBoard.MoveArchive.Count / 2} moves";

            GUI.RenameHeader(result);
            GUI.lbMoves.Items.Insert(0, $"Game Complete - {result}");
            GUI.lbMoves.Items.Insert(0, "");
            GUI._lbErrors.Items.Insert(0, $"Game Complete - {result}");

            if(!nGamesOn)
                GUI.SetWindow_NoGameInProgress();
            if(ViewBotsEnd)
                LiveBoard.UpdateBoardGUI();

            GameStat newGameStat = new GameStat(LiveBoard);
            GameStats.Add(newGameStat);

            if (winner.isBot)
                winner.BotBrain.AddRecord(newGameStat);
            if (loser.isBot)
                loser.BotBrain.AddRecord(newGameStat);
            
            //Bind the DataGrid to the Bot Records
            GUI.DG1.DataContext = BotController.GetBotRecords();

            if (nGamesOn)
                GUI._txtNGames.Text = (int.Parse(GUI._txtNGames.Text) - 1).ToString();

            GameOnView = false;
        }
    }
}


