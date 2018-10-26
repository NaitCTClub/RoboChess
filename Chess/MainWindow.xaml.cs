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

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Point ActiveCell; // Points to selected cell in [8,8], coordinates for nothing selected ->(-1, -1)  
        public Point TargetCell;
        public List<GamePiece> blackDead;
        public List<GamePiece> whiteDead;
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;
        public int[,] argArray = new int[8,8]; // Array for board comparison (canMove, etc)

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
            int x = 0, y = 0, count = 0;

            // Parsing x and y coordinated of button
            foreach (char c in ((Button)sender).Name)
            {
                if (count == 1)
                    int.TryParse(c.ToString(), out x);
                else if (count == 2)
                    int.TryParse(c.ToString(), out y);
                count++;
            }

            //Printing gamepiece for UI temporary effect
            PrintGamePiece(board.CellArray[x, y].CurrentGamePiece);

            // Find possible Moves for Game Piece
            ActiveCell = board.SelectCell(new Point(x,y));
            // Exit if invalid selection was made
            if (ActiveCell == new Point(-1, -1))
                return;
            // Find Array of moveable positions
            argArray = board.CanMove(ActiveCell);
            // Highlight possible moves for player in UI
            HighlightCells(argArray);

        }

        private void HighlightCells(int[,] moveableArray)
        {
            if (moveableArray is null)
                return;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    board.CellArray[x, y].CellColor(moveableArray[x, y]);
                }
            }
            
        }

        private void PrintGamePiece(GamePiece gp)
        {
            if (gp is null)
            {
                Title = "Empty space";
            }
            else
            {
                // if(cells[x,y].Equals(new King()))
                Title = " The piece is " + gp + " " +
                                            gp.PieceColor + " " +
                                            gp.ID;
            }
        }
    }
}
