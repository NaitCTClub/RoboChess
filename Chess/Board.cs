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
        public const int BOARD_SIZE = 8;

        // public GamePiece[,] cells = new GamePiece[8, 8];
        public Cell activeCell; // Points to selected cell in [8,8], coordinates for nothing selected ->(-1, -1)  
        List<Cell> possibleMoves;
        public Point targetCell;
        public List<GamePiece> blackDead = new List<GamePiece>();
        public List<GamePiece> whiteDead = new List<GamePiece>();
        public Player playerOne;
        public Player playerTwo;
        public Player WhosTurn;

        public Board()
        {
            for (int y = 0; y < BOARD_SIZE; y++)
                    for (int x = 0; x < BOARD_SIZE; x++)
                    {
                    cells[x, y] = new Cell(CELL_SIZE, CELL_SIZE, new Point(x, y));
                        cells[x, y].Piece = GamePiece.StartingPiece(new Point(x, y));
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
                    possibleMoves = cells[(int)cell.Position.X, (int)cell.Position.Y].Piece.PossibleMoves(cells);
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
            if (cell != activeCell)
            {
                if (possibleMoves.Contains(cell))
                    InvestigateMove(cell);
            }

            int x = (int)cell.Position.X;
            int y = (int)cell.Position.Y;

            if (cell.CellState == Cell.State.Neutral || cell == activeCell){
                //Check one above
                if (Utils.InRange(y + 1, 0, BOARD_SIZE-1) && possibleMoves.Contains(cells[x, y + 1]))
                    CanMove(cells[x, y + 1]);
                //Check below
                if (Utils.InRange(y - 1, 0, BOARD_SIZE-1) && possibleMoves.Contains(cells[x, y - 1]))
                    CanMove(cells[x, y - 1]);
                //Check left
                if (Utils.InRange(x - 1, 0, BOARD_SIZE-1) && possibleMoves.Contains(cells[x - 1, y]))
                    CanMove(cells[x - 1, y]);
                //Check right
                if (Utils.InRange(x + 1, 0, BOARD_SIZE-1) && possibleMoves.Contains(cells[x + 1, y]))
                    CanMove(cells[x + 1, y - 1]);
            }
        }

        // Investigates if a blind move is valid
        private void InvestigateMove(Cell blindCell)
        {
            // cell is empty
            if (blindCell.Piece is null)
            {
                // Set as possible NEUTRAL
                // Pawns are the exception with no diagonal neutral moves
                if (!(activeCell.Piece is Pawn && blindCell.Position.X != activeCell.Position.X))
                   blindCell.CellState = Cell.State.Neutral;
                else
                    blindCell.CellState = Cell.State.NoMove;
            }
            // cell is currently owned already by player
            else if (blindCell.Piece.PieceColor == WhosTurn.TeamColor)
            {
                // Do Nothing, impossible move
                blindCell.CellState = Cell.State.NoMove;
            }
            // cell is currently owned by Other player
            else
            {
                // set as possible ATTACK
                // Pawns are the exception with no straight attack moves
                if (!(activeCell.Piece is Pawn && blindCell.Position.X == activeCell.Position.X))
                    blindCell.CellState = Cell.State.Attack;
                else
                    blindCell.CellState = Cell.State.NoMove;
            }
        }
    }
}
