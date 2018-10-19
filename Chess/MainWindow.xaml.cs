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

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board board = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //Generate the game board
            board = new Board();
            //Assign the click events for the board.
            foreach (Cell cell in board.cells)
            {
                cell.CellButton.Click += Cell_Click;
                MyMainPanel.Children.Add(cell.CellButton);
            }
        }

        /// <summary>
        /// Handler for the click event on a cell in the board.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Cell cell = null;
            //Look for the button that was pressed.
            foreach (Cell c in board.cells)
                if (ReferenceEquals(c.CellButton, (Button)sender)) cell = c;

            // Find possible Moves for Game Piece
            HighlightCells();

            //Set the title bar with the cell status.
            Title = cell.ToString();
        }

        private void HighlightCells()
        {
            foreach (Cell cell in board.cells)
            {
                // Selected Cell
                if (board.activeCell != null && board.activeCell.Equals(cell))
                    cell.ChangeColor(Cell.CellColor.active);
                // Neutral move cell
                else if (cell.CellState == Cell.State.Neutral)
                    cell.ChangeColor(Cell.CellColor.neutral);
                // Attackable Cell
                else if (cell.CellState == Cell.State.Attack)
                    cell.ChangeColor(Cell.CellColor.attack);
                // Set to Default
                else
                {
                    if (((cell.Position.X + cell.Position.Y) % 2) == 0 || cell.Position.X + cell.Position.Y == 0)
                        cell.ChangeColor(Cell.CellColor.light);
                    else
                        cell.ChangeColor(Cell.CellColor.dark);
                }
            }
        }
    }
}
