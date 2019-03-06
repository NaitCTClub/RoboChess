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
using System.Drawing;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initiates all of board's children (Cells & GamePieces)
        Board board = new Board();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate the Cell to the UI of THIS MainWindow
            board.delButtons = LinkButton;
            board.GenerateBoard();
        }

        public void LinkButton(Cell c)
        {
            c.UIButton.Click += Cell_Click;
            MyMainPanel.Children.Add(c.UIButton);
        }

        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            // Find Cell associated to Button
            Cell focusCell = board.Cells.Find(b => ReferenceEquals(b.UIButton, (Button)sender));

            // Update Active Cell
            if (focusCell.Status == Cell.State.Default)
            {
                //Printing gamepiece for UI temporary effect
                Title = focusCell.ToString();

                // Set cells status for moveable positions
                board.CanMove(focusCell);

                board.ActiveCell = focusCell;

                UI_Message("Choose target Cell");
            }
            // Move GamePiece from Active Cell to Focus Cell
            else
            {
                Board.Move move = board.GamePieceMove(board.ActiveCell, focusCell);

                UI_Message($"Go {board.WhosTurn}");

                lbMoves.Items.Add($"{move.Piece} {move.From} To {move.To}");
            }
        }

        private void UI_Message(string message)
        {
            //Label tempMess = new Label { Name = "Test", Content = message, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            lHeader.Content = message;
        }
    }
}
