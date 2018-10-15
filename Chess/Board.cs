using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Chess
{
    public class Board
    {
        public GamePiece Contains { get; protected set; } //The game piece that owns the cell

        public GamePiece[,] cells = new GamePiece[8, 8];
        public Point activeCell;
        public Point targetCell;
        public GamePiece[] blackDead = new GamePiece[15];
        public GamePiece[] whiteDead = new GamePiece[15];
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;
        GamePiece gamePiece = new King();

        public Board()
        {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        cells[x, y] = gamePiece.StartingPiece(new Point(x, y));
                    }
                }
            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = playerTwo;
        }

        // Toggles active player after a move
        public void NextTurn()
        {
            if (WhosTurn == playerOne)
                WhosTurn = playerTwo;
            else
                WhosTurn = playerOne;
        }

        public int[,] SelectCell(Point cell)
        {

            if (!(cells[cell.X, cell.Y] is null))
            {
                // Only can select pieces that belong to that Player
                if (cells[cell.X, cell.Y].PieceColor == WhosTurn.TeamColor)
                {
                    activeCell = cell;
                    int[,] canMove = CanMove(cell);
                    return canMove;
                }
                else
                {
                    activeCell = default(Point);
                    return null;
                }
            }
            else
            {
                activeCell = default(Point);
                return null;
            }
        }

        // Highlights possible moves for selected Gamepiece
        //
        // NOTE requires a catch for pawns to prevent diagonal neutral movents 
        public int[,] CanMove(Point cell)
        {
            int[,] result = new int[8,8];
            //Get Bool array of all possible blind moves for specific Gamepiece
            bool[,] possibleMove = cells[cell.X, cell.Y].PossibleMove();

            // Will NEED to be changed
            // CanMove Array should be mapped from Gamepiece location outward
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    // Potential blind move
                    if(possibleMove[x,y])
                    {
                        // cell is empty
                        if(cells[x, y] == null)
                        {
                            // Set as possible
                            result[x, y] = 1;
                        }
                        // cell is currently owned already by player
                        else if(cells[x,y].PieceColor == WhosTurn.TeamColor)
                        {
                            // Do Nothing, impossible move
                        }
                        // cell is currently owned by Other player
                        else
                        {
                            // set as possible
                            result[x, y] = 2;
                        }
                    }
                }
            }
            // Return array of 0 = not possible move, 1 = possible empty cell, 2 = possible enemy cell
            return result;
        }
    }
}
