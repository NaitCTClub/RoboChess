using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing;
using Color = System.Drawing.Color;
using ChessTools;

namespace Chess
{
    /**
     * @brief The class that all game pieces are based off of.
     **/
    public abstract class GamePiece
    {
        public Color TeamColor { get; protected set; } //The color of the Team 
        public Point ID { get; protected set; }  //Identifier & starting location for the piece
        public Point Location { get; set; } //Current location
        public int moveCount { get; set; } = 0; // Used to check for legal Castling
        public bool isAlive { get; set; } //Indicates if the piece is still active on the board.
        public System.Windows.Controls.Image Img { get; protected set; }

        public delegate GamePiece DelChessMove(Color teamColor, Point loc);
        public static DelChessMove delChessMove = null;

        //Constructor for the game piece object. Initialize all parameters.
        protected GamePiece(Color teamColor, Point id)
        {
            //Set the internal members from the passed in values.
            TeamColor = teamColor;
            ID = id; //Default Location
            Location = id; //Current Location
            isAlive = true; //All pieces start out as active.
        }

        public abstract List<BlindMove> BlindMoves();

        public static GamePiece StartingPiece(Point cell)
        {
            Color color = Color.Red;

            GamePiece temp = null;

            //Top 2 Rows
            //BLACK PIECES
            if (cell.Y < 2)
            {
                color = Color.Black;
            }
            //Bottom 2 Rows
            //WHITE PIECES
            else if (cell.Y > 5)
            {
                color = Color.White;
            }
            else
                return null;

            //Pawn
            if (cell.Y == 1 || cell.Y == 6)
            {
                temp = new Pawn(color, cell);
            }
            //Rook
            else if (cell.X == 0 || cell.X == 7)
            {
                temp = new Rook(color, cell);
            }
            //Knight
            else if (cell.X == 1 || cell.X == 6)
            {
                temp = new Knight(color, cell);
            }
            //Bishop
            else if (cell.X == 2 || cell.X == 5)
            {
                temp = new Bishop(color, cell);
            }
            //Queen
            else if (cell.X == 3)
            {
                temp = new Queen(color, cell);
            }
            //King
            else if (cell.X == 4)
            {
                temp = new King(color, cell);
            }

            return temp;
        }

        public static void ChecknFlagEnpassant( List<Cell> cells, ChessMove move, bool undoMove = false)
        {
            //Check if Pawn that went 2 steps - (Enpassant)
            if (move.PieceMoved is Pawn && Math.Abs(move.From.ID.Y - move.To.ID.Y) == 2)
            {
                int step = (move.To.ID.Y - move.From.ID.Y) / 2;
                Point firstStep = new Point(0, step);
                Cell passedCell = cells.NextCell(move.From, firstStep);

                if(undoMove)
                    passedCell.enPassantPawn = null;
                else
                    passedCell.enPassantPawn = move.PieceMoved;

                // Set first step as Enpassant - (Link Pawn) remove next turn
            }
        }

        // Checks & Implements Pawn2Queen and return modified ChessMove
        // undoMove = true: Reverts Pawn2Queen
        public ChessMove Pawn2Queen(ChessMove move, Player owner, Board board, bool undoMove = false)
        {
            // Check if Pawn is in END ZONE
            if (!undoMove && this is Pawn && Math.Abs(this.ID.Y - move.To.ID.Y) == 6)
            {
                // Create new Queen & Replace Pawn on board
                Queen newQueen = (Queen)board.PromotionPieces.Find(gp => gp.TeamColor == owner.TeamColor && gp is Queen);
                board.PromotionPieces.Remove(newQueen);
                Console.WriteLine($"{this} Queened");

                newQueen.ID = this.ID;
                newQueen.Location = move.To.ID;
                move.To.Piece = newQueen;
                newQueen.isAlive = true;

                // Replace Pawn w/ Queen on Piece Collection
                owner.MyPieces.Remove(this);
                owner.MyPieces.Add(newQueen);

                // Add Queen to move's OtherInfo
                move.OtherInfo = newQueen;

                return move;
            }
            else if ( undoMove && move.PieceMoved is Pawn && Math.Abs(this.ID.Y - move.To.ID.Y) == 6)
            {
                if (!(move.OtherInfo is GamePiece pieceReplacement))
                    throw new ArgumentException("Can't find GamePiece in ChessMove's OtherInfo for Pawn2Queen replacement");

                // Remove Queen from Players pieces & Re-Insert Pawn
                pieceReplacement.ID = Point.Empty;
                pieceReplacement.isAlive = false;
                owner.MyPieces.Remove(pieceReplacement);
                owner.MyPieces.Add(move.PieceMoved);

                // Add Queen back into PromotionList
                board.PromotionPieces.Add(pieceReplacement);
                Console.WriteLine($"{this} Undo Queened");

                return move;
            }

            return move;
        }

        public override string ToString()
        {
            string color = this.TeamColor == Color.Black ? "Black" : "White";
            string piece = "";
            if (this is King)
                piece = "King";
            else if (this is Queen)
                piece = "Queen";
            else if (this is Bishop)
                piece = "Bishop";
            else if (this is Knight)
                piece = "Knight";
            else if (this is Rook)
                piece = "Rook";
            else if (this is Pawn)
                piece = "Pawn";

            return $"{color} {piece}";
        }
    }


}
