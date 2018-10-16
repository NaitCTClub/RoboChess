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
        public GamePiece[,] cells = new GamePiece[8, 8];
        public Point activeCell; // Points to selected cell in [8,8], coordinates for nothing selected ->(-1, -1)  
        public Point targetCell;
        public GamePiece[] blackDead = new GamePiece[15];
        public GamePiece[] whiteDead = new GamePiece[15];
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;
        GamePiece gamePiece = new King();

        public Board()
        {
            //cells[3,3] = new Rook(Color.White, new Point(3,3)); //Test subjects
            
            for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        cells[x, y] = gamePiece.StartingPiece(new Point(x, y));
                    }
                }
            playerOne = new Player(Color.White, "Player One");
            playerTwo = new Player(Color.Black, "Player Two");
            WhosTurn = playerOne;
        }

        // Toggles active player after a move
        public void NextTurn()
        {
            if (WhosTurn == playerOne)
                WhosTurn = playerTwo;
            else
                WhosTurn = playerOne;
        }

        public int[,] SelectCell(int x, int y)
        {
            int[,] canMove = new int[8,8];

            if (!(cells[x, y] is null))
            {
                // select piece that belong to active player
                if (cells[x, y].PieceColor == WhosTurn.TeamColor)
                {
                    activeCell = new Point(x,y);
                    canMove = CanMove(activeCell);
                }
                // non selectable (Selected Other player's gamepiece)
                else
                {
                    activeCell = new Point(-1,-1);
                }
            }
            // non selectable (Selected an empty cell)
            else
            {
                activeCell = new Point(-1, -1);
            }
            return canMove;
        }

        // Highlights possible moves for selected Gamepiece
        //
        // NOTE requires a catch for pawns to prevent diagonal neutral movents 
        public int[,] CanMove(Point cell)
        {
            int[,] result = new int[8,8];
            string activeGP = gpToStr(cells[cell.X, cell.Y]);

            //Get Bool array of all possible blind moves for specific Gamepiece
            bool[,] possibleMove = cells[cell.X, cell.Y].PossibleMove();


            // Will NEED to be changed
            // CanMove Array should be mapped from Gamepiece location outward
            //
            Point testPoint = new Point();
            int xDir; // Test Direction x
            int yDir; // Test Direction y
            // testing possible moves from active gamepiece outward
            // 8 Possible Directions to test
            for (int i = 1; i <= 8; i++)
            {
                // Up & Left
                if (i == 1)
                {
                    xDir = -1;
                    yDir = -1;
                }
                // Up
                else if(i == 2)
                {
                    xDir = 0;
                    yDir = -1;
                }
                // Up & Right
                else if (i == 3)
                {
                    xDir = 1;
                    yDir = -1;
                }
                // Right
                else if (i == 4)
                {
                    xDir = 1;
                    yDir = 0;
                }
                // Down & Right
                else if (i == 5)
                {
                    xDir = 1;
                    yDir = 1;
                }
                // Down
                else if (i == 6)
                {
                    xDir = 0;
                    yDir = 1;
                }
                // Down & left
                else if (i == 7)
                {
                    xDir = -1;
                    yDir = 1;
                }
                // Left
                else
                {
                    xDir = -1;
                    yDir = 0;
                }

                testPoint.X = cell.X + xDir;
                testPoint.Y = cell.Y + yDir;
                while (testPoint.X <= 7 && testPoint.X >= 0 && testPoint.Y <= 7 && testPoint.Y >= 0)
                {
                    if (possibleMove[testPoint.X, testPoint.Y])
                    {
                        result[testPoint.X, testPoint.Y] = InvestigateMove(testPoint.X, testPoint.Y, activeGP);
                        // Stop once you hit a filled cell, only Knight can bypass
                        if (result[testPoint.X, testPoint.Y] != 1 && activeGP != "Knight")
                            break;
                    }
                    else
                        break;
                    testPoint.X += xDir;
                    testPoint.Y += yDir;
                }

            }
            // Return int array
            // 0 = NOT possible move
            // 1 = possible NEUTRAL move
            // 2 = possible ATTACK move
            return result;
        }

        // Investigates if a blind move is valid
        private int InvestigateMove(int x, int y, string activeGP)
        {
            GamePiece blindGP = cells[x, y];
            // cell is empty
            if (blindGP is null)
            {
                // Set as possible NEUTRAL
                // Pawns are the exception with no diagonal neutral moves
                if (!(activeGP == "Pawn" && x != activeCell.X))
                    return 1;
                else
                    return 0;
            }
            // cell is currently owned already by player
            else if (cells[x, y].PieceColor == WhosTurn.TeamColor)
            {
                // Do Nothing, impossible move
                return 0;
            }
            // cell is currently owned by Other player
            else
            {
                // set as possible ATTACK
                // Pawns are the exception with no straight attack moves
                if (!(activeGP == "Pawn" && x == activeCell.X))
                    return 2;
                else
                    return 0;
            }
        }

        public string gpToStr(GamePiece gPiece)
        {
            if (gPiece is Pawn)
                return "Pawn";
            else if (gPiece is Rook)
                return "Rook";
            else if (gPiece is Knight)
                return "Knight";
            else if (gPiece is Bishop)
                return "Bishop";
            else if (gPiece is Queen)
                return "Queen";
            else if (gPiece is King)
                return "King";
            else
                return null;
        }

        public int InRange(int value)
        {
            int max = 7;
            int min = 0;
            int result = Math.Max(Math.Min(value, max), min);
            return result;
        }
    }
}
