using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessTools;

namespace Chess
{
    public class Player
    {
        public Color TeamColor { get; protected set; } //The color of the gamie piece.
        public string Name { get; protected set; }  //Identifier for the piece
        public List<GamePiece> Pieces { get; set; } = new List<GamePiece>();

        public Player(Color color, string name)
        {
            TeamColor = color;
            Name = name;
        }

        public override string ToString()
        {
            string player;

            if (TeamColor == Color.White)
                player = "Player One";
            else
                player = "Player Two";

            return player;
        }
    }
}
