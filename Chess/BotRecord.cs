using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessTools;

namespace Chess
{
    public class BotRecord
    {
        public BotController BotBrain { get; set; } // **Not Used
        public int CheckMates { get; set; } = 0;
        public int StaleMates { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int MovesAvg { get; set; } = 0;
    }

    public static class Extensions
    {
        public static void AddRecord(this BotController bot, GameStat results)
        {
            bot.Record.BotBrain = bot;

            if (results.Result == GameResult.BoardFlipped)
                return; //Game ended prematurely

            if (results.Result == GameResult.Draw || results.Winner is null)
            {
                bot.Record.Draws++; // Draw
            }
            else if (results.Winner.BotBrain is null)
            {
                bot.Record.Losses++; // Winner is a Human
            }
            else if (results.Winner.BotBrain.GetType().Equals(bot.GetType())) // I am the Winner!!
            {
                if (results.Result == GameResult.CheckMate)
                    bot.Record.CheckMates++;
                else if (results.Result == GameResult.StaleMate)
                    bot.Record.StaleMates++;
            }
            else
            {
                bot.Record.Losses++; // Loser...
            }

            bot.Record.MovesAvg = (bot.Record.MovesAvg + (results.BoardArchive.Moves_Index / 2)) / 2;
        }
    }
}
