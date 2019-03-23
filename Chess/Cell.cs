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
        private static int _Height = 88;
        private static int _Width = 88;
        public bool isVirtual { get; private set; } = false;
        public GamePiece Piece { get; set; } = null;
        public Button UIButton { get; protected set; }
        public Condition Status { get; set; } = Condition.Default; // Only Used for Human player interaction
        public GamePiece enPassantPawn { get; set; } // Identifies Pawn that used first move to 'pass' this cell (the first of two cells forward)

        // Constructor
        public Cell(int x, int y) // Live Cells
        {
            ID = new Point(x, y);
            Piece = GamePiece.StartingPiece(new Point(x, y)); // <= **in the future it would be Nice to move to directly at Board Gen
            UIButton = new Button
            {
                Width = _Width,
                Height = _Height,
                Name = $"P{ID.X}{ID.Y}"
            };

            ChangeState(Condition.Default);
        }

        public Cell(Cell c) // Virtual Cells
        {
            isVirtual = true;
            ID = c.ID;
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

        public void CellColor()
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
            else if (Status == Condition.Castling)
                UIButton.Background = neutralMove;
            // Default
            else
                // Sequence for creating the Board's pattern in the UI
                if (((this.ID.Y + this.ID.X) % 2) == 0 || this.ID.Y + this.ID.X == 0)
                    UIButton.Background = lightCell;
                else
                    UIButton.Background = darkCell;
        }

        public SolidColorBrush CellColorTemp()
        {
            // Neutral move cell
            if (Status == Condition.Neutral)
                return neutralMove;
            // Attackable Cell
            else if (Status == Condition.Attack)
                return attackMove;
            // Active Cell
            else if (Status == Condition.Active)
                return activeCell;
            // Enpassant Cell
            else if (Status == Condition.enPassant)
                return attackMove;
            else if (Status == Condition.Castling)
                return neutralMove;
            // Default
            else
                // Sequence for creating the Board's pattern in the UI
                if (((this.ID.Y + this.ID.X) % 2) == 0 || this.ID.Y + this.ID.X == 0)
                return lightCell;
            else
                return darkCell;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell arg))
                return false;

            return this.ID.Equals(arg.ID);
        }

        public override string ToString()
        {
            return $"[{ID.X}, {ID.Y}]";
        }
    }

    //public static class UI_Update
    //{
    //    public static SolidColorBrush ColorBack(this Button btn, List<Cell> cells)
    //    {
    //        Cell relatedCell = cells.Find(c => ReferenceEquals(c.UIButton, btn));

    //        return relatedCell.CellColorTemp();
    //    }
    //}
}
