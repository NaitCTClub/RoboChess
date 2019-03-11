using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ChessTools;
using static ChessTools.Library;

namespace Chess
{
    class Bot
    {
        private Board MainBoard;
        public Player Me;
        public Color TeamColor = Color.Black;
        private Random _rando = new Random();


        public Bot(Board board, Player player, Color teamColor)
        {
            TeamColor = teamColor;
            MainBoard = board;
            Me = player;
        }

        public ChessMove MyTurn() // Controller calls this to activate Bots turn
        {
            ChessMove newMove = new ChessMove();

            List<ChessMove> lsOfMoves =  FindAllPossibleMoves();

            if (lsOfMoves.Count == 0)
                throw new ArgumentException("Couldn't Find any moves! Forgot to confirm CheckMate?");
            else
                return lsOfMoves[_rando.Next(0, lsOfMoves.Count)];
        }

        public List<ChessMove> FindAllPossibleMoves() // Tool for finding ALL ChessMoves Bot's GamePieces can do
        {
            List<ChessMove> lsOfMoves = new List<ChessMove>();

            foreach (GamePiece piece in Me.Pieces.FindAll(p => p.isAlive))
            {
                List<ChessMove> moves = MainBoard.PossibleMoves(piece);
                foreach(ChessMove move in moves)
                {
                    lsOfMoves.Add(move);
                }
                
            }

            List<ChessMove> attackMoves = lsOfMoves.FindAll(mve => !(mve.PieceCaptured is null));
            if (attackMoves.Count > 0)
            {
                attackMoves.OrderBy(mve => mve.PieceCaptured);
                return attackMoves;
            }
            else
                return lsOfMoves;
        }


    }
}
