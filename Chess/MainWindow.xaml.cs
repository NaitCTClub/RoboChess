using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using ChessTools;
using Condition = ChessTools.Condition;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }



        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Pass the command wand off to Controller
            controller = new Controller(this);

            // Looks at all Subclasses of BotController and ADDS them to UI's Player "Brain" RadioButton
            foreach (Type bot in typeof(BotController).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BotController))))
            {
                _playerOneBots.Children.Add(new RadioButton() { Content = $"{bot.Name}" });
                _playerTwoBots.Children.Add(new RadioButton() { Content = $"{bot.Name}" });
            }

            //controller.NewGame("Human", "Human");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controller.nGamesOn = false;
            controller.gameOn = false;
            controller.nGamesPause = false;
            Thread.Sleep(500); // Wait for possible Bot threads to be killed
            controller.Close();
        }


        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Human Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////

        private void btnNewGame(object sender, RoutedEventArgs e)
        {
            if (controller.nGamesOn || controller.gameOn) // Button = 'Cancel'
            {
                controller.CancelGame();
                return;
            }

            // Button = 'New Game'
            
            // Get the checked radio Button that defines Player's 'Brain'
            string playerOneBrain = (string)_playerOneBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;
            string playerTwoBrain = (string)_playerTwoBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;

            SetWindow_ActiveGameinProgress();
            controller.NewGame(playerOneBrain, playerTwoBrain);

            //if (!controller.BotsBattle()) // Starts bots vs bot
            //        _lbStats.Items.Add($"Error with Bot: {controller.WhosTurn}");
        }

        private void _btnNGames_Click(object sender, RoutedEventArgs e)
        {
            if (controller.nGamesOn || controller.gameOn) // N Matches already in progress
            {
                if (controller.nGamesPause) // -> Continue
                {
                    controller.nGamesPause = false;
                    SetWindow_ActiveGameinProgress();
                    return;
                }
                else // -> Pause
                {
                    controller.nGamesPause = true;
                    SetWindow_PausedGameinProgress();
                    return;
                }
            }

            // New N Matches!
            string playerOneBrain = (string)_playerOneBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;
            string playerTwoBrain = (string)_playerTwoBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;

            Controller.nGame nGame = new Controller.nGame();
            nGame.bot1 = playerOneBrain;
            nGame.bot2 = playerTwoBrain;

            if(!int.TryParse(_txtNGames.Text, out nGame.gameCount))
            {
                MessageBox.Show("Please enter valid number of Rounds");
                return;
            }

            // Good to GO!
            SetWindow_ActiveGameinProgress();
            controller.PlayNGamesThreadStart(nGame);
        }

        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            // Find Cell associated to Button
            Cell focusCell = controller.LiveBoard.Cells.Find(b => ReferenceEquals(b.UIButton, (Button)sender));

            // Pass to Controller
            controller.BoardClick(focusCell);
        }

        private void UI_btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (controller.UndoMove())
                _lbHeader.Text = "Undo move";
            else
                _lbHeader.Text = "Unable to Undo move";
        }

        private void UI_btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (controller.RedoMove())
                _lbHeader.Text = "Redo move";
            else
                _lbHeader.Text = "Unable to redo move";
        }

        // User Option for watching Bot Game
        private void _chkWatchBotGame_Checked(object sender, RoutedEventArgs e)
        {
            if (controller is null)
                return; // Protect against value change on Initial startup

            if ((bool)_chkWatchBotGame.IsChecked)
            {
                _chkAnimateMoves.IsChecked = true; // Add Animation option
                _chkAnimateMoves.Visibility = Visibility.Visible;

                controller.RefreshBoard = true;
            }
            else
            {
                _chkAnimateMoves.IsChecked = false; // Also Remove Animation
                _chkAnimateMoves.Visibility = Visibility.Hidden;

                controller.RefreshBoard = false;
            }
        }

        private void _gameSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (controller is null)
                return; // Protect against value change on Initial startup

            controller.speed = (int)_gameSpeed.Value;
        }

        private void _chkAnimateMoves_Checked(object sender, RoutedEventArgs e)
        {
            if (controller is null)
                return; // Protect against value change on Initial startup

            if ((bool)_chkAnimateMoves.IsChecked)
                controller.animate = true;
            else
                controller.animate = false;
        }

        private void _chkWatchBotEndGame_Checked(object sender, RoutedEventArgs e)
        {
            if (controller is null)
                return; // Protect against value change on Initial startup

            if ((bool)_chkWatchBotEndGame.IsChecked)
                controller.ViewBotsEnd = true;
            else
                controller.ViewBotsEnd = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Thread GUI Update
        //
        /////////////////////////////////////////////////////////////////////////////////////

        public void RenameHeader(string message)
        {
            //Label tempMess = new Label { Name = "Test", Content = message, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            _lbHeader.Text = message;
        }

        // Highlight GamePiece Movement
        public void DispatchInvoke_UIAnimate(Cell focus, Condition condition)
        {
            Dispatcher.Invoke(new Action(delegate () { AnimateMove(focus, condition); }));
        }

        private void AnimateMove(Cell focus, Condition condition)
        {
            focus.ChangeState(condition);
        }

        public void UpdateGameInfo(string player1Brain, string player2Brain)
        {
            _playerOneTeam.Content = new System.Windows.Controls.Image()
            {
                Source = new BitmapImage(new Uri("Resources/WhiteKing.png", UriKind.Relative))
            };
            _playerOneTeam.Background = new SolidColorBrush(Colors.Gray) { Opacity = 0.8 };
            _playerOneBrain.Text = player1Brain;
            _playerTwoTeam.Content = new System.Windows.Controls.Image()
            {
                Source = new BitmapImage(new Uri("Resources/BlackKing.png", UriKind.Relative))
            };
            _playerTwoTeam.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.8 };
            _playerTwoBrain.Text = player2Brain;
        }

        public void DispatchInvoke_RemoveMove(ChessMove move)
        {
            Dispatcher.Invoke(new Action(delegate () { LbMoves_RemoveMove(move); }));
        }

        public void LbMoves_RemoveMove(ChessMove move)
        {
            lbMoves.Items.RemoveAt(0);
            UpdateWindow();
        }

        public void DispatchInvoke_AddMove(ChessMove move)
        {
            Dispatcher.Invoke(new Action(delegate () { LbMoves_AddMove(move); }));
        }

        public void LbMoves_AddMove(ChessMove move)
        {
            if(move.OtherInfo is Cell) // Enpassant move
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Enpass Captured: {move.PieceCaptured}");
            else if (move.OtherInfo is ChessMove move2) // Castling
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Castling w/ {move2.PieceMoved} {move2.From} to {move2.To}");
            else if (move.OtherInfo is GamePiece gp && move.PieceCaptured is null) // Promotion
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Promoted TO {gp}");
            else if (move.OtherInfo is GamePiece gp2) // Promotion w/ Capture
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Promoted TO {gp2}");
            else if (move.PieceCaptured is null) // Neutral Move
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To}");
            else // Attack Move
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Captured: {move.PieceCaptured}");
        }

        public void UpdateWindow()
        {
            _txtMoveCount.Text = $"Total Moves: {controller.LiveBoard.Moves_Index + 1}";

            controller.LiveBoard.UpdateBoardGUI();

            if (controller.LiveBoard.Result != GameResult.InProgress)
                return;

            // Notify whos turn it is
            if (controller.WhosTurn.isChecked)
                RenameHeader($"Check! Go {controller.WhosTurn}");
            else
                RenameHeader($"Go {controller.WhosTurn}");

            if (controller.nGamesPause)
                _lbHeader.Text = $"GAME PAUSED\n{_lbHeader.Text}";

        }

        public void SetWindow_NoGameInProgress()
        {
            _GameOnDisplay.Visibility = System.Windows.Visibility.Hidden;
            _btnNewGame.Content = "New Game";
            _btnNGames.Content = "Play N Games";
        }

        public void SetWindow_ActiveGameinProgress()
        {
            _GameOnDisplay.Visibility = System.Windows.Visibility.Visible;
            _btnNewGame.Content = "Cancel Game";
            _btnNGames.Content = "Pause Game";
        }

        public void SetWindow_PausedGameinProgress()
        {
            _btnNGames.Content = "Continue Game";
        }

        //public ChessMove DispatchInvoke_Pawn2Queen(ChessMove move)
        //{
        //    move = Dispatcher.Invoke(new Action(delegate () { Pawn2Queen(move); }));
        //}

        //private GamePiece Pawn2Queen(Color teamColor, Point ID)
        //{
        //    return new Queen(teamColor, ID);
        //}
    }
}

