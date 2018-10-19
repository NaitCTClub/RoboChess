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

        Board board = new Board();
        public Button[,] bCellArray = new Button[8,8];

        SolidColorBrush darkCell = new SolidColorBrush(Colors.Gray) { Opacity = 0.8 };
        SolidColorBrush lightCell = new SolidColorBrush(Colors.LightGray) { Opacity = 0.8 };
        SolidColorBrush activeCell= new SolidColorBrush(Colors.Yellow) { Opacity = 0.8 };
        SolidColorBrush neutralMove = new SolidColorBrush(Colors.Blue) { Opacity = 0.8 };
        SolidColorBrush attackMove = new SolidColorBrush(Colors.Red) { Opacity = 0.8 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            for (int y = 0; y < 8 ; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cell cTemp = new Cell(x, y);
                    Button bTemp = cTemp.CreateButton();
                    bTemp.Click += Cell_Click;
                    MyMainPanel.Children.Add(bTemp);
                    bCellArray[x, y] = bTemp;
                }
            }

        }

        private void Cell_Click(object sender, RoutedEventArgs e)
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

            // Find possible Moves for Game Piece
            HighlightCells(board.SelectCell(x,y));


            if (!(board.cells[x, y] == null))
            {
                Title = board.cells[x, y].GetType().ToString();


                // if(cells[x,y].Equals(new King()))
                Title += " The piece is " + ((GamePiece)board.cells[x, y]).isAlive + " " + 
                                            ((GamePiece)board.cells[x, y]).PieceColor + " " + 
                                            ((GamePiece)board.cells[x, y]).ID;
            }
            else
            {
                Title = "Empty space";
            }

        }

        private void HighlightCells(int[,] moveableArray)
        {
            if (!(moveableArray is null))
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        // Selected Cell-
                        if (board.activeCell.X == x && board.activeCell.Y == y)
                            bCellArray[x, y].Background = activeCell;
                        // Neutral move cell
                        else if (moveableArray[x, y] == 1)
                            bCellArray[x, y].Background = neutralMove;
                        // Attackable Cell
                        else if (moveableArray[x, y] == 2)
                            bCellArray[x, y].Background = attackMove;
                        // Set to Default
                        else
                        {
                            if (((y + x) % 2) == 0 || y + x == 0)
                                bCellArray[x, y].Background = lightCell;
                            else
                                bCellArray[x, y].Background = darkCell;
                        }
                    }
                }
            }
        }
    }
}
