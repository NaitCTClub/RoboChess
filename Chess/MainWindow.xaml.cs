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
using ChessTools;

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
            board.ClearCellsStatus();
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

            if (focusCell.Status == CellState.Default)
            {
                // Clear statuses
                board.ClearCellsStatus();

                //Printing gamepiece for UI temporary effect
                Title = focusCell.ToString();

                // check if legal select
                if (!board.SelectCell(focusCell)) return;

                // Set cells status for moveable positions
                board.PossibleMoves(focusCell);

                // Highlight possible moves for player in UI
                board.HighlightCells();

                board.ActiveCell = focusCell;
            }
            // Move GamePiece
            else if (focusCell.Status == CellState.Neutral)
            {
                // Move Active GamePiece
                focusCell.Piece = board.ActiveCell.Piece;
                focusCell.Piece.Location = focusCell.ID;
                board.ActiveCell.Piece = null;

                board.ClearCellsStatus();
                board.ActiveCell = null;

                board.NextTurn();
            }
            // Attack w/ GamePiece
            else if (focusCell.Status == CellState.Enemy)
            {
                // Destroy Enemy GamePiece, Move Active GamePiece
                if (board.WhosTurn.TeamColor == Color.Black)
                    board.WhiteDead.Add(focusCell.Piece);
                else
                    board.BlackDead.Add(focusCell.Piece);

                focusCell.Piece = board.ActiveCell.Piece;
                focusCell.Piece.Location = focusCell.ID;
                board.ActiveCell.Piece = null;

                board.ClearCellsStatus();
                board.ActiveCell = null;

                board.NextTurn();
            }
        }
    }
}
