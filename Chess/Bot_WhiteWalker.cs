//=======================================================================================================================================
// ROBOCHESS - BOT - Skeleton Code
// Created by Jon Klassen
// Last Modified: March 12, 2019
//
// Description: This is where the brain of the operation lives. While I have supplied a few helper functions, the rest is up to you. If
//              you have any questions or theres anything I missed, please let me know on GitHub https://github.com/NaitCTClub/RoboChess
//              This is still a work in the making so be patient.
//
// Crucial Functions you NEED to use
///     MyTurn() -                    its located here and it communicates with the Controller. Only use *ChessMoves* from PossibleMoves()
///     MainBoard.PossibleMoves() -   returns all legal moves [including making sure the king is not vulnerable after the move]
///     
// Crucial Knowledge
///     Board Navigation          -   All Cells in MainBoard are given Point coordinates on property ID wit [0,0] on top left and [7,7] on bottom right
///                                   Example: MainBoard.Cells[0].ID = [0,0]
///                                            MainBoard.Cells[63].ID = [7,7]
///                                                     OR easier via    
///                                            MainBoard.Cells.GetCell(Point) - return Cell associated to that point
///                                            MainBoard.Cells.NextCell(Reference Cell, Point) - returns a cell in certain direction 
///                                                                         Up - new Point( 0, -1 )
///                                                                         down -        ( 0,  1 )
///                                                                         Left  -       (-1,  0 )
///                                                                         Right  -      ( 1,  0 )
///                                                                         Diagonal -    ( 1,  1 )
///                                                                         
///     Struct *ChessMove*        -   Instructions and details for a GamePiece move 
///                                   [Cell From, Cell To, GamePiece moved, GamePiece captured, Condition MoveType]
///     Struct *Condition*        -   Represents the *ChessMove* type
///                                   [Attack][Neutral][EnPassant]
///     Class *Board*             -   Contains the list of Cells, players, whosturn, gamepieces
///     Class *Cells*             -   Squares on the Board - another way to acquire GamePiece locations, Uses *Point* for coordinates via ID      
///     Class *GamePiece*         -   Base class for subclasses of all Piece types (King, Queen, etc)
///                                                                         
///     
//  Helpful Knowledge
///     MainBoard.PlayerOne       -   Used for looking up Opponent moves if "Me == MainBoard.PlayerOne" for example     
///     MainBoard.PlayerTwo       -     
///     MainBoard.MovePiece()     -   Allows you to see the board one step ahead - Careful, it currently changes the MainBoard So....
///     MainBoard.UndoMovePiece() -   This Must Be used each time after using MovePiece()
//=======================================================================================================================================

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
    public class Bot_WhiteWalker : BotController
    {
        private Random _rando = new Random();
        private MoveValue _moveValue = new MoveValue();

        //              Add your global variables here

        public Bot_WhiteWalker() : base() {}
        public Bot_WhiteWalker(Board board, Player player) : base(board, player)
        {
        }


        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Your Bot Mouth Goes Here               *Only change the inside code
        //
        /////////////////////////////////////////////////////////////////////////////////////
        public override ChessMove MyTurn() // [DONT CHANGE] Controller calls this to activate Bots turn
        {
            List<ChessMove> lsOfMoves =  GetAllMoves(); // Example

            if (lsOfMoves.Count == 0) // If you have no Moves then CheckMate!
                return new ChessMove(null, null, null, null, Condition.Illegal);

            return GetTheBloodiest(lsOfMoves);            
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        //                  Your Bot Brain Goes Here                *Example Code provided
        //
        ///////////////////////////////////////////////////////////////////////////////////// 

        //***Let your strategic brain go nutz here!! All of this below is just **Example*** code

        // Example Function - Returns a List of All legal *ChessMove* for your *GamePiece*'s
        public List<ChessMove> GetAllMoves()
        {
            List<ChessMove> lsOfMoves = new List<ChessMove>();

            // Iterate through all your gamepieces
            foreach (GamePiece piece in Me.MyPieces.FindAll(p => p.isAlive)) // Usefull, no point in moving a DEAD GamePiece
            {
                List<ChessMove> moves = MainBoard.PossibleMoves(piece);

                lsOfMoves.AddRange(moves);
            }

            return lsOfMoves;
        }

        // Example Function - Returns a single *ChessMove*
        private ChessMove GetTheBloodiest(List<ChessMove> lsMoves)
        {
            List<ChessMove> attackMoves = lsMoves.FindAll(m => m.MoveType == Condition.Attack);

            if (attackMoves.Count > 0)
            {
                _moveValue._sortType = MoveValue.SortType.Descending;
                _moveValue._sortBy = MoveValue.SortBy.GamePieceCaptured;

                attackMoves.Sort(_moveValue); // Uses MoveValue Class to sort the list

                return attackMoves.First();
            }

            return lsMoves[_rando.Next(0, lsMoves.Count)];
        }


    }

    internal class MoveValue : IComparer<ChessMove>
    {
        public enum SortType { Ascending, Descending};
        public enum SortBy { GamePieceMoved, GamePieceCaptured};
        public SortType _sortType { get; set; }
        public SortBy _sortBy { get; set; }

        public MoveValue()
        {
            _sortType = SortType.Descending;
            _sortBy = SortBy.GamePieceCaptured;
        }

        public int Compare(ChessMove mve1, ChessMove mve2)
        {
            int direction = _sortType == SortType.Ascending ? 1 : -1;
            GamePiece p1;
            GamePiece p2;

            if (_sortBy == SortBy.GamePieceCaptured)
            {
                p1 = mve1.PieceCaptured;
                p2 = mve2.PieceCaptured;
            }
            else
            {
                p1 = mve1.PieceMoved;
                p2 = mve2.PieceMoved;
            }


            if (p1 is null && p2 is null)
                return 0;
            if (p1 is null)
                return -1 * direction;
            if (p2 is null)
                return direction;

            if (p1.GetType() == p2.GetType())
                return 0; // Same GamePiece type (Pawn == Pawn)

            if (p1 is King) // Highest Value
                return direction;
            if(p1 is Queen) // Second Highest Value
            {
                if (p2 is Bishop || p2 is Knight || p2 is Rook || p2 is Pawn)
                    return direction;
                else
                    return -1 * direction;
            }
            if (p1 is Rook) // Third Highest Value
            {
                if (p2 is Bishop || p2 is Knight || p2 is Pawn)
                    return direction;
                else
                    return -1 * direction;
            }
            if (p1 is Bishop) // Tied 4th with Knight
            {
                if (p2 is Pawn)
                    return direction;
                if (p2 is Knight)
                    return 0;
                else
                    return -1 * direction;
            }
            if (p1 is Knight) // Tied 4th with Bishop
            {
                if (p2 is Pawn)
                    return direction;
                if (p2 is Bishop)
                    return 0;
                else
                    return -1 * direction;
            }
            else
                return -1 * direction; // Pawn is on the bottom, no point in looking for smaller pieces :( awe
        }
    }
}
