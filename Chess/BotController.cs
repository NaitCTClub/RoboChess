using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessTools;

using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Chess
{
    public abstract class BotController
    {
        public static List<BotController> BotBrains { get; set; }
        public Player Me { get; protected set; }
        public Player Opponent { get; protected set; }
        public Board VirtualBoard { get; set; }
        public BotRecord Record { get; private set; } = new BotRecord();

        public BotController()
        {}

        static BotController()
        {
            BotBrains = GetBotBrains();
        }

        public abstract ChessMove MyTurn();
        public abstract GamePiece Promotion();

        // Create/Update Virtual World
        public void UpdateBrain(Board liveBoard, Player player)
        {
            VirtualBoard = new Board(liveBoard);

            Me = liveBoard.PlayerOne == player ? VirtualBoard.PlayerOne : VirtualBoard.PlayerTwo;
            Opponent = liveBoard.PlayerOne == player ? VirtualBoard.PlayerTwo : VirtualBoard.PlayerOne;
        }

        // Creates and returns new Bot 
        // - Matches a string 'Name' to find matching Bot Type 'Name'
        public static BotController MatchBotBrain(string botName)
        {
            foreach (BotController brain in BotBrains)
            {
                if (botName.Equals(brain.GetType().Name))
                    return brain;
            }

            return null; // Whoops couldn't find a matching Brain
        }

        // Creates and returns list of available Bots
        public static List<BotController> GetBotBrains()
        {
            List<BotController> result = new List<BotController>();
            foreach (Type bot in typeof(BotController).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(BotController))))
            {
                result.Add((BotController)Activator.CreateInstance(bot)); // Initiate Bot (arg is second param if needed)
            }

            return result;

            // ===== Could be used if getting bots from dll ========
            //var subclasses =
            //    from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //    from type in assembly.GetTypes()
            //    where type.IsSubclassOf(typeof(BotController))
            //    select type;
        }

        public static ObservableCollection<BotRecord> GetBotRecords()
        {
            ObservableCollection<BotRecord> result = new ObservableCollection<BotRecord>();

            foreach (BotController brain in BotBrains)
            {
                result.Add(brain.Record);
            }

            return result; 
        }
    }
}
