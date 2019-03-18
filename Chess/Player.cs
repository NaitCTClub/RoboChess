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
        public List<GamePiece> MyPieces { get; private set; } = new List<GamePiece>();
        public bool isChecked { get; private set; } = false;
        public bool isBot { get; set; } = false;
        public BotController BotBrain { get; private set; }

        public Player(Color color, string name)
        {
            TeamColor = color;
            Name = name;            
        }

        public bool BotThePlayer(string botName, Board mainBoard)
        {
            if (botName is null || mainBoard is null)
                return false;
            
            BotBrain = GetBotBrain(botName, mainBoard);
            isBot = true;

            return true;
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
            if (!board.IsSafe(MyPieces.Find(gp => gp is King).Location, this))
                isChecked = true;
            else
                isChecked = false;

            return isChecked;
        }

        private BotController GetBotBrain(string botName, Board board)
        {
            foreach (Type bot in typeof(BotController).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BotController))))
            {
                if (botName.Equals(bot.Name))
                {
                    object[] arg = { board, this };
                    return (BotController)Activator.CreateInstance(bot, arg);
                }
            }

            return null; // Whoops couldn't find a matching Brain


            // ===== Could be used if getting bots from dll ========
            //var subclasses =
            //    from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //    from type in assembly.GetTypes()
            //    where type.IsSubclassOf(typeof(BotController))
            //    select type;
        }
    }
}
