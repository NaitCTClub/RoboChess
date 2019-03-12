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
        public string Name { get; protected set; }  // Possible For name change
        public List<GamePiece> MyPieces { get; private set; } = new List<GamePiece>();
        public bool isChecked { get; private set; } = false;
        public bool isBot { get; set; } = false;

        public Player(Color color, string name)
        {
            TeamColor = color;
            Name = name;
        }

        public override string ToString()
        {
            if (TeamColor == Color.White)
                return "Player One";
            else
                return "Player Two";
        }

        public bool AmIChecked(Board board)
        {
            if (!board.IsSafe(MyPieces.Find(gp => gp is King), this))
                isChecked = true;
            else
                isChecked = false;

            return isChecked;
        }
    }
}
