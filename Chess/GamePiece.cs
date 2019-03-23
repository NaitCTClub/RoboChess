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
        public Point ID { get; protected set; }  //Game start location for the piece
        public Color TeamColor { get; protected set; } //The color of the Team 
        public Point Location { get; set; } //Current location
        public int Value { get; set; } // Used by Bots
        public bool isAlive { get; set; } //Indicates if the piece is still active on the board.
        public bool isVirtual { get; private set; }
        public int moveCount { get; set; } = 0; // Used to check for legal Castling
        public System.Windows.Controls.Image Img { get; protected set; }

        protected GamePiece (Color teamColor, Point id) // LiveBoard
        {
            TeamColor = teamColor;
            ID = id; //Default Location
            Location = id; //Current Location
            isAlive = true; //All pieces start out as active.
        }
        protected GamePiece (GamePiece piece) //Virtual
        {
            ID = piece.ID;
            TeamColor = piece.TeamColor;
            Location = piece.Location;
            isAlive = piece.isAlive;
            isVirtual = true;
            moveCount = piece.moveCount;
        }
        protected GamePiece() { } // Used for Promotion

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

        public static GamePiece VirtualPiece(GamePiece piece)
        {
            if (piece is King)
                return new King(piece);
            if (piece is Queen)
                return new Queen(piece);
            if (piece is Rook)
                return new Rook(piece);
            if (piece is Bishop)
                return new Bishop(piece);
            if (piece is Knight)
                return new Knight(piece);
            else
                return new Pawn(piece);
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
        public ChessMove Promotion(ChessMove move, Player owner, Board board)
        {
            if (!(this is Pawn)) // Only Applies to Pawns
                return move;
            if (Math.Abs(this.ID.Y - this.Location.Y) != 6) // Check if Pawn is in END ZONE
                return move;


            // Create new Queen & Replace Pawn on board
            GamePiece promotionPiece;
            if (owner.isBot)
                promotionPiece = owner.BotBrain.Promotion();
            else
            {
                PopUp pop = new PopUp(owner);
                pop.Owner = board.controller.GUI;
                var result = pop.ShowDialog();

                promotionPiece = pop.ReturnGamePiece;
            }

            if (board.isVirtual)
            {
                promotionPiece.isVirtual = true;
                promotionPiece.TeamColor = owner.TeamColor; // to cover promotion constructor
            }
            else
            {
                promotionPiece = board.PromotionPieces.Find(gp => gp.TeamColor == owner.TeamColor && gp.GetType().Equals(promotionPiece.GetType())); // Prevents thread from crashing
                board.PromotionPieces.Remove(promotionPiece);
            }

            Console.WriteLine($"{this} Queened");

            // Promotion
            promotionPiece.ID = this.ID;
            promotionPiece.Location = move.To.ID;
            promotionPiece.isAlive = true;
            move.To.Piece = promotionPiece;

            // Replace Pawn w/ Queen on Piece Collection
            owner.MyPieces.Remove(this);
            owner.MyPieces.Add(promotionPiece);

            board.AllPieces.Remove(this);
            board.AllPieces.Add(promotionPiece);

            // Add Queen to move's OtherInfo
            move.OtherInfo = promotionPiece;

            return move;
        }

        public void UndoPromotion(ChessMove move, Player owner, Board board)
        {
            if (!(this is Pawn)) // Only Applies to Pawns move
                return;
            if (Math.Abs(this.ID.Y - move.To.ID.Y) != 6) // Check if promotion occured
                return;            
            if (!(move.OtherInfo is GamePiece pieceReplacement))
                throw new ArgumentException("Can't find GamePiece in ChessMove's OtherInfo for Pawn2Queen replacement");

            if (!board.isVirtual) // Not necessary in Virtual
            {
                // Add Queen back into PromotionList
                board.PromotionPieces.Add(pieceReplacement);
                // Remove Queen from Players pieces & Re-Insert Pawn
                pieceReplacement.ID = Point.Empty;
                pieceReplacement.isAlive = false;
            }

            owner.MyPieces.Remove(pieceReplacement);
            owner.MyPieces.Add(move.PieceMoved);

            board.AllPieces.Remove(pieceReplacement);
            board.AllPieces.Add(move.PieceMoved);

            Console.WriteLine($"{this} Undo Queened");
            
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

        public override int GetHashCode()
        {
            return 1;
        }

        // Matches Gamepieces that have same: ID and Piece Type (ie Rook [0,7])
        public override bool Equals(object obj) 
        {
            if (!(obj is GamePiece arg))
                return false;

            return ID.Equals(arg.ID) && this.GetType().Equals(arg.GetType());
        }
    }


}
