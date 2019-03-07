using System;
using System.Drawing;

namespace ChessTools
{
    /// <summary>
    ///  States for Cells when Player is viewing possible moves
    /// </summary>
    public enum CellState { Default, Active, Neutral, Enemy, enPassant };
    public enum MoveType { Illegal, Default, Neutral, Attack, Safe, enPassant};
    
    /// <summary>
    /// Properties for POTENTIAL moves of a GamePiece
    /// </summary>
    public struct BlindMove
    {
        public Point Direction;     // x & y direction
        public int Limit;           // # of times it can move in that direction
        public CellState Condition;     // Special conditions that need to be met
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir">x & y direction</param>
        /// <param name="limit"># of times it can move in that direction (-1 = unlimited)</param>
        /// <param name="condition">Special conditions that need to be met</param>
        public BlindMove(Point dir, int limit, CellState condition = CellState.Default)
        {
            Direction = dir;
            Limit = limit;
            Condition = condition;
        }
    }

    /// <summary>
    /// Properties for the POSSIBLE moves of a GamePiece
    /// </summary>
    public struct CanMove
    {
        public Point Location;   // location on board
        public MoveType Type;      // style of move (Neutral, Attack, enPassant)
        public CanMove(Point location, MoveType type)
        {
            Location = location;
            Type = type;
        }
    }



    public class Library
    {
    }
    
}
