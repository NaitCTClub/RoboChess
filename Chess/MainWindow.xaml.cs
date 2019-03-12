using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void _dispTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            //foreach(Cell cellb in controller.LiveBoard.Cells)
            //{
            //    cellb.UIButton.Background = cellb.CellColorTemp();
            //}

            // Forcing the CommandManager to raise the RequerySuggested event
            //CommandManager.InvalidateRequerySuggested();
        }

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate the Cell to the UI of THIS MainWindow
            //board.delButtons = LinkButton;
            //board.GenerateBoard();

            // Pass the command wand off to Controller
            controller = new Controller(this);
            controller.LiveBoard.delButtons = LinkButton;
            controller.GenerateGame();
            //controller.LiveBoard.GenerateBoard();

            //_dispTimer = new DispatcherTimer();
            //_dispTimer.Tick += _dispTimer_Tick;
            //_dispTimer.Interval = new TimeSpan(0, 0, 1);
            //_dispTimer.Start();
        }

        public void LinkButton(Cell c)
        {
            c.UIButton.Click += Cell_Click;
            MyMainPanel.Children.Add(c.UIButton);
        }

        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            // Find Cell associated to Button
            Cell focusCell = controller.LiveBoard.Cells.Find(b => ReferenceEquals(b.UIButton, (Button)sender));

            // Pass to Controller
            controller.BoardClick(focusCell);

            //UI_btnUndo.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
        }

        public void RenameTitle(string message)
        {
            Title = message;
        }

        public void RenameHeader(string message)
        {
            //Label tempMess = new Label { Name = "Test", Content = message, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            lHeader.Content = message;
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

        public void AnimateMove(ChessMove move)
        {
            List<SolidColorBrush> colors = new List<SolidColorBrush>
            {
                new SolidColorBrush(Colors.Red) { Opacity = 0.8 },
                new SolidColorBrush(Colors.Yellow) { Opacity = 0.8 },
                new SolidColorBrush(Colors.Orange) { Opacity = 0.8 },
                new SolidColorBrush(Colors.Green) { Opacity = 0.8 },
                new SolidColorBrush(Colors.Blue) { Opacity = 0.8 },
                new SolidColorBrush(Colors.Purple) { Opacity = 0.8 }
            };

            //move.From.UIButton.Refresh();

            for (int i = 3; i < 6; i++)
            {
                move.To.ChangeState(move.MoveType);
                move.To.UIButton.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
                System.Threading.Thread.Sleep(200);
                move.To.ChangeState(Condition.Default);
                move.To.UIButton.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
                //move.To.UIButton.Background = colors[i];
                System.Threading.Thread.Sleep(100);
            }

            //move.From.UIButton.Refresh();
            //for (int i = 0; i < 3; i++)
            //{
            //    move.From.ChangeState(move.MoveType);
            //    move.From.UIButton.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
            //    System.Threading.Thread.Sleep(100);
            //    //move.From.UIButton.Background = colors[i];
            //    move.From.ChangeState(Condition.Default);
            //    move.From.UIButton.Dispatcher.Invoke((Action)(() => { }), DispatcherPriority.Render);
            //    System.Threading.Thread.Sleep(100);
            //}
        }

        private void UI_btnBotBattle_Click(object sender, RoutedEventArgs e)
        {
            if (!controller.BotsBattle()) // Starts bots vs bot
                UI_Stats.Items.Add($"Error with Bot: {controller.WhosTurn}");
        }
    }

    //public static class ExtensionMethods
    //{
    //    private static Action EmptyDelegate = delegate() { };

    //    public static void Refresh(this UIElement uiElement)
    //    {
    //        uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
    //    }
    //}
}
