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
    public class Cell
    {
        // Testing access modifiers, dont read too much into the different styles
        // Jon Klassen
        public static readonly SolidColorBrush darkCell = new SolidColorBrush(Colors.Gray) { Opacity = 0.8 };
        public static readonly SolidColorBrush lightCell = new SolidColorBrush(Colors.LightGray) { Opacity = 0.8 };
        public static SolidColorBrush activeCell = new SolidColorBrush(Colors.Yellow) { Opacity = 0.8 };
        public static SolidColorBrush neutralMove = new SolidColorBrush(Colors.Blue) { Opacity = 0.8 };
        public static SolidColorBrush attackMove = new SolidColorBrush(Colors.Red) { Opacity = 0.8 };

        public Point ID { get; protected set; }
        public int Height { get; protected set; }
        public int Width { get; protected set; }
        public string Name { get; protected set; }


        public Cell(int x, int y)
        {
            Width = 44;
            Height = 44;
            Name = $"C{x}{y}";
            ID = new Point(x, y);
        }

        public Button CreateButton()
        {
            Button newCell = new Button();
            newCell.Width = Width;
            newCell.Height = Height;
            newCell.Name = Name;

            // Sequence for creating the Board's pattern in the UI
            if (((this.ID.Y + this.ID.X) % 2) == 0 || this.ID.Y + this.ID.X == 0)
                newCell.Background = lightCell;
            else
                newCell.Background = darkCell;

            return newCell;
        }

    }
}
