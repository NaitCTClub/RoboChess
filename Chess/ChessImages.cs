using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Color = System.Drawing.Color;
using ChessTools;

namespace Chess
{
    public static class ChessImages
    {
        /// <summary>
        /// Team White
        /// </summary>

        public static Image White_King { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whiteKing.png", UriKind.Relative))
        };
        public static Image White_Queen { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whiteQueen.png", UriKind.Relative))
        };
        public static Image White_Bishop { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whiteBishop.png", UriKind.Relative))
        };
        public static Image White_Knight { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whiteKnight.png", UriKind.Relative))
        };
        public static Image White_Rook { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whiteRook.png", UriKind.Relative))
        };
        public static Image White_Pawn { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/whitePawn.png", UriKind.Relative))
        };

        /// <summary>
        /// Team Black
        /// </summary>

        public static Image Black_King { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackKing.png", UriKind.Relative))
        };
        public static Image Black_Queen { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackQueen.png", UriKind.Relative))
        };
        public static Image Black_Bishop { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackBishop.png", UriKind.Relative))
        };
        public static Image Black_Knight { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackKnight.png", UriKind.Relative))
        };
        public static Image Black_Rook { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackRook.png", UriKind.Relative))
        };
        public static Image Black_Pawn { get; private set; } = new Image()
        {
            Source = new BitmapImage(new Uri("Resources/blackPawn.png", UriKind.Relative))
        };
    }
}
