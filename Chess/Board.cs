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
        public Cell[,] cells = new Cell[8, 8];

        private const int CELL_SIZE = 44;
         
       // public GamePiece[,] cells = new GamePiece[8, 8];
        public Cell activeCell; // Points to selected cell in [8,8], coordinates for nothing selected ->(-1, -1)  
        public Point targetCell;
        public List<GamePiece> blackDead = new List<GamePiece>();
        public List<GamePiece> whiteDead = new List<GamePiece>();
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
                    cells[x, y] = new Cell(CELL_SIZE, CELL_SIZE, new System.Windows.Point(x, y));
                        cells[x, y].Piece = GamePiece.StartingPiece(new Point(x, y));
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

        public void SelectCell(Cell cell)
        {

            if (!(cell.Piece is null))
            {
                // select piece that belong to active player
                if (cell.Piece.PieceColor == WhosTurn.TeamColor)
                {
                    activeCell = cell;
                    CanMove(activeCell);
                }
                // non selectable (Selected Other player's gamepiece)
                else
                {
                    activeCell = null;
                }
            }
            // non selectable (Selected an empty cell)
            else
            {
                activeCell = null;
            }
        }

        // Highlights possible moves for selected Gamepiece
        //
        // NOTE requires a catch for pawns to prevent diagonal neutral movents 
        public void CanMove(Cell cell)
        {
            List<Cell> result = new List<Cell>();

            //Get Bool array of all possible blind moves for specific Gamepiece
            bool[,] possibleMove = cells[(int)cell.Position.X, (int)cell.Position.Y].Piece.PossibleMove();
            GamePiece activeGP = cell.Piece;

            // Will NEED to be changed
            // CanMove Array should be mapped from Gamepiece location outward
            //
            int xDir; // Test Direction x
            int yDir; // Test Direction y
            // testing possible moves from active gamepiece outward
            // 8 Possible Directions to test
            for(int x = (int)cell.Position.X -1; x <= (int)cell.Position.X + 1; x++ )
                for (int y = (int)cell.Position.Y - 1; y <= (int)cell.Position.Y + 1; y++)
                    for (int i = 1; i <= 8; i++)
            {
                        Point testPoint = new Point(x, y);
                        while (testPoint.X <= 7 && testPoint.X >= 0 && testPoint.Y <= 7 && testPoint.Y >= 0)
                {
                    if (possibleMove[x, y])
                    {

                        result[x, y] = InvestigateMove(cells[x, y], activeGP);
                        // Stop once you hit a filled cell, only Knight can bypass
                        if (result[x, y] != 1 && !(activeGP is Knight))
                            break;
                    }
                    else
                        break;
                    testPoint.X += xDir;
                    testPoint.Y += yDir;
                }
            }
        }



        // Investigates if a blind move is valid
        private int InvestigateMove(Cell blindCell, GamePiece activeGP)
        {
            // cell is empty
            if (blindCell.Piece is null)
            {
                // Set as possible NEUTRAL
                // Pawns are the exception with no diagonal neutral moves
                if (!(activeGP is Pawn && blindCell.Position.X != activeCell.Position.X))
                    return 1;
                else
                    return 0;
            }
            // cell is currently owned already by player
            else if (blindCell.Piece.PieceColor == WhosTurn.TeamColor)
            {
                // Do Nothing, impossible move
                return 0;
            }
            // cell is currently owned by Other player
            else
            {
                // set as possible ATTACK
                // Pawns are the exception with no straight attack moves
                if (!(activeGP is Pawn && blindCell.Position.X == activeCell.Position.X))
                    return 2;
                else
                    return 0;
            }
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
