# RoboChess
a Chess game that bots play

We are long way from AI computers, but that is the end goal.

At this point you are able to play 1v1 with another human on the same pc, 
Or against a bot of your making.. Sooooo exciting.

GETTING STARTED
1. Download the RoboChess Folder
2. Open the Chess.sln file in the first layer of the folder.
3. Edit the Bot_YourBot.cs File to your liking (Read the comments)
4. Run the program to play with your new Bot!

If you have any questions or requests for this project please let me know via username: jklassen

Thanks for reading!
Jon Klassen


//=======================================================================================================================================
//  ROBOCHESS                                                BOT - Skeleton Code
//                                                          Created by Jon Klassen
//                                                      Last Modified: March 30, 2019
//
// Description: This is where the brain of the operation lives. You are supplied a *VirtualBoard* that contains all related objects for
//              the board's environment. I have supplied a few helper functions and the rest is up to you. If
//              you have any questions or theres anything I missed, please let me know on GitHub https://github.com/NaitCTClub/RoboChess
//              This is still a work in progress so please be patient.
//
// Crucial Methods you NEED to use
///     MyTurn() ---------------------  Handles Board's request for a move. Return a single legal *ChessMove* that was
///                                     acquired from PossibleMoves()
///     Promotion() ------------------  Handles Board's request for promoting a pawn. Return a GamePiece with default constructor 
///                                     [ex. new Queen();]                   
///     VirtualBoard.PossibleMoves() -  Returns a list of legal ChessMoves for a single GamePiece.
///                                     [including making sure the king is not vulnerable after the move]
///     
// Crucial Knowledge
///     Board Navigation -------------  The *VirtualBoard* is broken up into a list of 64 Cells that contain:
///                                         *ID* - Point Coordinates
///                                         *Piece* - current GamePiece resident
///     [0,0] [1,0] [2,0]               All Cells in VirtualBoard are given Point coordinates on property *ID* with [0,0] on top left 
///     [0,1] [1,1] [2,1]               and [7,7] on bottom right
///     [0,2] [1,2] [2,2]                   
///                                     Example: VirtualBoard.Cells[0].ID = [0,0]
///                                              VirtualBoard.Cells[8].ID = [0,1]
///                                                         OR easier via    
///                                              VirtualBoard.Cells.GetCell( new Point(7,7) ).ID = [7,7]
///                                              VirtualBoard.Cells.NextCell(Reference Cell, Point) - returns a cell in certain direction 
///                                                                         Up - new Point( 0, -1 )
///                                                                         down -        ( 0,  1 )
///                                                                         Left  -       (-1,  0 )
///                                                                         Right  -      ( 1,  0 )
///                                                                         Diagonal -    ( 1,  1 )
///                                                                         
///     Struct *ChessMove* ---------  Instructions and details for a GamePiece move 
///                                   [Cell From, Cell To, GamePiece moved, GamePiece captured, Condition MoveType, object OtherInfo]
///     Struct *Condition* ---------  Represents the *ChessMove* type
///                                   [Attack][Neutral][EnPassant][Castling]
///     Class *Board* --------------  Contains the list of Cells, players, whosturn, gamepieces
///     Class *Cells* --------------  Squares on the Board - another way to acquire GamePiece locations, Uses *Point* for coordinates via ID      
///     Class *GamePiece -----------  Base class for subclasses of all Piece types (King, Queen, etc)
///                                                                         
///     
//  Helpful Knowledge
///     RefreshMemory() ------------- Handles a call to refresh your variables that reference the LiveBoard, Players, and cells. 
            // Each turn you have new references. So you need to update your variables.
///     VirtualBoard.PlayerOne ------ Used for looking up Opponent moves if "Me == LiveBoard.PlayerOne" for example     
///     VirtualBoard.PlayerTwo            
///     VirtualBoard.MovePiece() ---- Allows you to see the board one step ahead. Returns modified ChessMove if pawn is promoted
///                                   *Careful, if Pawn is moved forward you MUST update *ChessMove* to the returned *ChessMove*
///     VirtualBoard.UndoMovePiece()- Self explanatory here. BUT NOTE:
///                                   Move & UndoMove are must maintain valid stack sequence. MovePiece is the 'PUSH' and UndoMovePiece is the 'PULL'.
///                                   First 'PUSH' is the Last 'PULL' (Like a stack of plates, first one off is the last one back on)
///                                   Last MovePiece is then First UndoMovePiece.
///                                   
///     Me.isChecked ---------------- Bool, True if you are Checked. *Will not update with your MovePiece(), UndoMovePiece()
///     Me.MyPieces ----------------- Your List of *GamePiece*
///     Me.TeamColor ---------------- Color: White/Black
///     Opponent.MyPieces ----------- Opponent's List of *GamePiece*
///     Opponent.TeamColor ---------- Color: White/Black     
///                                   
//=======================================================================================================================================
