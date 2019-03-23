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
using System.Windows.Shapes;
using System.Drawing;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;


namespace Chess
{
    /// <summary>
    /// Interaction logic for PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        public GamePiece ReturnGamePiece { get; set; }

        public PopUp(Player who)
        {
            InitializeComponent();
            //this.WindowStyle = WindowStyle.ToolWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;

            string teamColor;
            if (who.TeamColor == Color.White)
                teamColor = "white";
            else
                teamColor = "black";

            _btnKnight.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"Resources/{teamColor}Knight.png", UriKind.Relative))
            };
            _btnBishop.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"Resources/{teamColor}Bishop.png", UriKind.Relative))
            };
            _btnRook.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"Resources/{teamColor}Rook.png", UriKind.Relative))
            };
            _btnQueen.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"Resources/{teamColor}Queen.png", UriKind.Relative))
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void _btnKnight_Click(object sender, RoutedEventArgs e)
        {
            ReturnGamePiece = new Knight();
            Close();
        }

        private void _btnBishop_Click(object sender, RoutedEventArgs e)
        {
            ReturnGamePiece = new Bishop();
            Close();
        }

        private void _btnRook_Click(object sender, RoutedEventArgs e)
        {
            ReturnGamePiece = new Rook();
            Close();
        }

        private void _btnQueen_Click(object sender, RoutedEventArgs e)
        {
            ReturnGamePiece = new Queen();
            Close();
        }
    }
}
