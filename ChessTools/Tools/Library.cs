using System;
using System.Collections.Generic;
using System.Drawing;
using Chess;

namespace ChessTools
{
    /// <summary>
    ///  States for Cells when Player is viewing possible moves
    /// </summary>
    public enum Condition { Illegal, Default, Active, Neutral, Attack, enPassant };
    
    /// <summary>
    /// Properties for POTENTIAL moves of a GamePiece
    /// </summary>
    public struct BlindMove
    {
        public Point Direction;     // x & y direction
        public int Limit;           // # of times it can move in that direction
        public Condition Condition;     // Special conditions that need to be met
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir">x & y direction</param>
        /// <param name="limit"># of times it can move in that direction (-1 = unlimited)</param>
        /// <param name="condition">Special conditions that need to be met</param>
        public BlindMove(Point dir, int limit, Condition condition = Condition.Default)
        {
            Direction = dir;
            Limit = limit;
            Condition = condition;
        }
    }

    /// <summary>
    /// Properties for the POSSIBLE moves of a GamePiece
    /// </summary>
    public struct ChessMove
    {
        public Cell From;
        public Cell To;
        public GamePiece PieceMoved;
        public GamePiece PieceCaptured;
        public Condition MoveType;
        public Cell OtherCell;

        public ChessMove(Cell from, Cell to, GamePiece pieceMoved,  GamePiece pieceCap, Condition moveType, Cell otherCell = null)
        {
            From = from;
            To = to;
            PieceMoved = pieceMoved;
            PieceCaptured = pieceCap;
            MoveType = moveType;
            OtherCell = otherCell;
        }
    }

    public static class Library
    {
        //===========================================================================
        //                          Cell extensions
        //===========================================================================

        public static Cell NextCell(this List<Cell> cells, Cell focus, Point Direction)
        {
            if (cells is null || focus is null)
                return null;

            Point newLocation = AddPoints(focus.ID, Direction);

            if (InBoardsRange(newLocation))
                return cells[PointIndex(newLocation)];
            else
                return null;
        }

        public static Cell GetCell(this List<Cell> cells, Point location)
        {
            if (InBoardsRange(location))
                return cells[PointIndex(location)];
            else
                return null;
        }


        //===========================================================================
        //                          Point to Cell Helpers
        //===========================================================================

        /// <summary>
        /// Tests to see if Point is within Boards scope
        /// </summary>
        /// <param name="test"></param>
        /// <returns>True if its inside the Board</returns>
        public static bool InBoardsRange(Point test)
        {
            int max = 7;
            int min = 0;

            return test.X <= max && test.X >= min &&
                    test.Y <= max && test.Y >= min;
        }

        // Finds out if two Points are in the same lane on Board
        public static bool SameLane(Point p1, Point p2)
        {
            if (p1.X == p2.X) //  Row
                return true;
            if (p1.Y == p2.Y) //  Column 
                return true;
            if (Math.Abs(p1.X - p2.X) == Math.Abs(p1.Y - p2.Y)) // Diagonal
                return true;

            return false;
        }

        public static Point AddPoints(Point p1, Point p2)
        {
            p1.X += p2.X;
            p1.Y += p2.Y;

            return p1;
        }

        /// <summary>
        ///     Allows finding cell of specific point in Board.Cells List
        /// </summary>
        /// <param name="cell"> X/Y of cell on board</param>
        /// <returns>Index of the cell in List of Cells for the Board</returns>
        public static int PointIndex(Point cell)
        {
            return cell.X + (cell.Y * 8);
        }
    }
    
}
