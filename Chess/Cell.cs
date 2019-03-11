using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using ChessTools;

namespace Chess
{
    public class Cell
    {
        // Jon Klassen
        //public enum State { Default, Active, Neutral, Enemy };

        private static readonly SolidColorBrush darkCell = new SolidColorBrush(Colors.Gray) { Opacity = 0.8 };
        private static readonly SolidColorBrush lightCell = new SolidColorBrush(Colors.LightGray) { Opacity = 0.8 };
        private static readonly SolidColorBrush activeCell = new SolidColorBrush(Colors.Yellow) { Opacity = 0.8 };
        private static readonly SolidColorBrush neutralMove = new SolidColorBrush(Colors.Blue) { Opacity = 0.8 };
        private static readonly SolidColorBrush attackMove = new SolidColorBrush(Colors.Red) { Opacity = 0.8 };

        public Point ID { get; protected set; }
        private static int _Height = 44;
        private static int _Width = 44;
        public GamePiece Piece { get; set; }
        public Button UIButton { get; protected set; }
        public Condition Status { get; set; } = Condition.Default; // Only Used for Human player interaction
        public GamePiece enPassantPawn { get; set; } // Identifies Pawn that used first move to 'pass' this cell (the first of two cells forward)

        // Constructor
        public Cell(int x, int y)
        {
            ID = new Point(x, y);
            Piece = GamePiece.StartingPiece(new Point(x, y));
            UIButton = new Button
            {
                Width = _Width,
                Height = _Height,
                Name = $"P{ID.X}{ID.Y}"
                //,Content = Piece.Img
        };

            ChangeState(Condition.Default);
        }


        public void ChangeState(Condition state)
        {
            // Update state enum
            Status = state;
            // Update Background color
            CellColor();

            // Update Gamepiece Image
            if (!(Piece is null))
                UIButton.Content = Piece.Img;
            else
                UIButton.Content = null;
        }

        private void CellColor()
        {
            // Neutral move cell
            if (Status == Condition.Neutral)
                UIButton.Background = neutralMove;
            // Attackable Cell
            else if (Status == Condition.Attack)
                UIButton.Background = attackMove;
            // Active Cell
            else if (Status == Condition.Active)
                UIButton.Background = activeCell;
            // Enpassant Cell
            else if (Status == Condition.enPassant)
                UIButton.Background = attackMove;
            // Default
            else
                // Sequence for creating the Board's pattern in the UI
                if (((this.ID.Y + this.ID.X) % 2) == 0 || this.ID.Y + this.ID.X == 0)
                    UIButton.Background = lightCell;
                else
                    UIButton.Background = darkCell;
        }

        public override string ToString()
        {
            string result;

            if (Piece is null)
            {
                result = "Empty space";
            }
            else
            {
                // if(cells[x,y].Equals(new King()))
                result = " The piece is " + Piece + " " +
                                            Piece.TeamColor + " " +
                                            Piece.ID;
            }
            return result;
        }
    }
}
