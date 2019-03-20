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
        // Initiates all of board's children (Cells & GamePieces)
        //Board board = new Board();
        Controller controller;
        DispatcherTimer _dispTimer;
        bool Test = false;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

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

            //controller.LiveBoard.delButtons = LinkButton;
            controller.NewGame("Human", "Human");
            //GamePiece.delChessMove = Pawn2Queen;

            //PopUp pop = new PopUp();
            //pop.Owner = this;
            //pop.ShowDialog();

            if (MessageBoxResult.Yes == MessageBox.Show(this, "New Game?", "Welcome to RoboChess!", MessageBoxButton.YesNo))
            {
                MessageBox.Show("Cool!");
            }
            else
                MessageBox.Show("Ok...");
        }


        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Human Player Interaction
        //
        /////////////////////////////////////////////////////////////////////////////////////

        private void btnNewGame(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            
            // Get the checked radio Button that defines Player's 'Brain'
            string playerOneBrain = (string)_playerOneBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;
            string playerTwoBrain = (string)_playerTwoBots.Children.OfType<RadioButton>().ToList().Find(rb => (bool)rb.IsChecked).Content;

            controller.NewGame(playerOneBrain, playerTwoBrain);

            //if (!controller.BotsBattle()) // Starts bots vs bot
            //        _lbStats.Items.Add($"Error with Bot: {controller.WhosTurn}");
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
                lHeader.Content = "Undo move";
            else
                lHeader.Content = "Unable to Undo move";
        }

        private void UI_btnRedo_Click(object sender, RoutedEventArgs e)
        {
            if (controller.RedoMove())
                lHeader.Content = "Redo move";
            else
                lHeader.Content = "Unable to redo move";
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

            lHeader.Content = message;
        }

        // Update Move Archive & Messages
        public void DispatchInvoke_UIUpdate(ChessMove move)
        {
            Dispatcher.Invoke(new Action(delegate () { UpdateBoard(move); }));
        }
        private void UpdateBoard(ChessMove move)
        {
            // Show Move Archive -- **Need to add ability to remove undo moves
            if (move.PieceCaptured is null)
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To}");
            else
                lbMoves.Items.Insert(0, $"{move.PieceMoved} {move.From} To {move.To} | Captured: {move.PieceCaptured}");

            _txtMoveCount.Text = $"Move Count: {controller.MoveArchive.Count()}"; // **Error since MoveArchive is total moves including current undos

            // Notify whos turn it is
            if (controller.WhosTurn.isChecked)
            {
                if (controller.LiveBoard.isCheckMate())
                    controller.CheckMate();
                else
                    RenameHeader($"Check! Go {controller.WhosTurn}");

            }
            else
                RenameHeader($"Go {controller.WhosTurn}");

            controller.LiveBoard.HighlightBoard();
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

