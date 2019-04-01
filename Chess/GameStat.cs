using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessTools;

namespace Chess
{
    public class GameStat
    {
        public Board BoardArchive { get; set; }
        public GameResult Result { get; set; }
        public Player Winner { get; set; }
        public DateTime Date { get; set; }

        public GameStat(Board archiveBoard)
        {
            BoardArchive = archiveBoard;

            King kingOne = (King)archiveBoard.PlayerOne.MyPieces.Find(gp => gp is King);
            King kingTwo = (King)archiveBoard.PlayerTwo.MyPieces.Find(gp => gp is King);

            Result = archiveBoard.Result;

            if (Result == GameResult.CheckMate || Result == GameResult.StaleMate)
            {
                if (archiveBoard.WhosTurn == archiveBoard.PlayerTwo)
                    Winner = archiveBoard.PlayerOne;
                else
                    Winner = archiveBoard.PlayerTwo;
            }
            else
                Winner = null;

            Date = DateTime.Now;
        }
    }

}
