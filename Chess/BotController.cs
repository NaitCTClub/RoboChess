using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessTools;

namespace Chess
{
    public  abstract class BotController
    {
        public Player Me { get; protected set; }
        public Player Opponent { get; protected set; }
        public Board MainBoard { get; protected set; }                                                    

        public BotController() // Reference Constructor
        {}

        public BotController(Board board, Player player) // Creation Constructor
        {
            MainBoard = board;
            Me = player;
            Me.isBot = true;     

            Opponent = MainBoard.playerOne == Me ? MainBoard.playerTwo : MainBoard.playerOne;
        }

        public abstract ChessMove MyTurn();
    }
}
