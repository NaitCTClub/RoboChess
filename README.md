# RoboChess
Chess game that bots play

We are long way from AI computers, but that is the end goal.

At this point you are able to play 1v1 with another human on the same pc. 
- Next goal is to have ability to play on seperate computers over wifi.

Currently there is 5 main classes that communicate in a object oriented manner:
  MainWindow.cs - Controls the UI front for the user
  Controller.cs - The brains behind how everything interacts (Currently using MainWindow.cs to accomplish this)
  Board.cs - Contains the players & the cells (8x8)
  Cells.cs - Contains the location of the cell, the GamePiece in posession of the cell, and its appearence attributes
  GamePiece.cs - Contains the baseclass of the specific game piece and its owner, subclasses: King.cs, Queen.cs, Bishop.cs, etc.
  
If you have trouble having rights to create your own branch of this project please let me know via jon.klassen@outlook.com

thanks for reading!
Jon Klassen
