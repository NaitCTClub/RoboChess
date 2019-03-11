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
        Controller controller;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyMainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Delegate the Cell to the UI of THIS MainWindow
            board.delButtons = LinkButton;
            board.GenerateBoard();

            // Pass the command wand off to Controller
            controller = new Controller(this, board);
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

            // Pass to Controller
            controller.BoardClick(focusCell);
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
    }
}
