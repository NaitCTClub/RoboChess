using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chess
{
    public class Cell
    {
        public enum CellColor
        {
            dark,
            light,
            active,
            neutral,
            attack
        }

        public enum State
        {
            NoMove,
            Neutral,
            Attack
        }
        public Button CellButton { get; } = new Button();

        public GamePiece Piece { get; set; } = null;
        public System.Drawing.Point Position { get; }

        public State CellState { get; set; } = State.NoMove;

        public bool Visited { get; set; } = false;
        public Cell(int height, int width, System.Drawing.Point pos)
        {
            Position = pos;

            CellButton.Height = height;
            CellButton.Width = width;
            CellButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            CellButton.VerticalAlignment = VerticalAlignment.Stretch;
            if ((pos.Y + pos.X) % 2 == 0 || pos.Y + pos.X == 0)
                ChangeColor(CellColor.light);
            else
                ChangeColor(CellColor.dark);
        }

        /// <summary>
        /// Replace the default ToString() method.
        /// </summary>
        /// <returns>The status of the cell.</returns>
        public override string ToString()
        {
            if (Piece != null)
                return Piece.ToString() + " The piece is " + Piece.isAlive + " " +
                                            Piece.PieceColor + " " + Piece.ID;

            return "Empty Space";
        }

        /// <summary>
        /// Change the background color of the cell.
        /// </summary>
        /// <param name="color">The desired color of the cell.</param>
        public void ChangeColor(CellColor color)
        {
            switch (color)
            {
                case CellColor.dark:
                    CellButton.Background = new SolidColorBrush(Colors.Gray) { Opacity = 0.8 };
                    break;
                case CellColor.light:
                    CellButton.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.8 };
                    break;
                case CellColor.active:
                    CellButton.Background = new SolidColorBrush(Colors.Yellow) { Opacity = 0.8 };
                    break;
                case CellColor.neutral:
                    CellButton.Background = new SolidColorBrush(Colors.Blue) { Opacity = 0.8 };
                    break;
                case CellColor.attack:
                    CellButton.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.8 };
                    break;
            }
        }
    }
}
