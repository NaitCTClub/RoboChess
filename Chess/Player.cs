using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using ChessTools;

namespace Chess
{
    public class Player
    {
        public Color TeamColor { get; protected set; } //The color of the gamie piece.
        public string Name { get; protected set; }  // Possible For name change
        public List<GamePiece> MyPieces { get; set; } = new List<GamePiece>();
        public bool isChecked { get; set; } = false;
        public bool isBot { get; set; } = false;
        public bool isVirtual { get; private set; } = false; // Flags instances that are in Bots World
        public BotController BotBrain { get; private set; }

        public Player(Color color, string name) // Live Player
        {
            TeamColor = color;
            Name = name;            
        }

        public Player(Player livePlayer) // Virtual Player
        {
            isVirtual = true;
            isBot = true;
            BotBrain = livePlayer.BotBrain;
            TeamColor = livePlayer.TeamColor;
            Name = livePlayer.Name;
        }

        public bool BotThePlayer(string botName, Board liveBoard)
        {
            if (botName is null || liveBoard is null)
                return false;

            BotBrain = BotController.MatchBotBrain(botName);
            isBot = true;

            return true;
        }

        public void UpdateBot(Board liveBoard)
        {
            if (!isBot)
                return;

            BotBrain.UpdateBrain(liveBoard, this);
        }

        public override string ToString()
        {
            if (TeamColor == Color.White)
                return "Player One";
            else
                return "Player Two";
        }
    }
}
