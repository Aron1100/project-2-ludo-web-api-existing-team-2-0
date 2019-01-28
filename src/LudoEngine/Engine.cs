using System;
using System.Collections.Generic;

namespace Ludo.Engine
{
    public class Piece
    {
        public int position;

        public int id;

        public Piece(int id)
        {
            this.id = id;
        }

        public bool IsInYard()
        {
            return position == 0;
        }

        public bool IsInGoal()
        {
            return position > 62;
        }

        public int StepsUntilGoal()
        {
            return 63 - position;
        }

        public void Move(int steps)
        {
            position += steps;
        }
    }

    public class Player
    {
        Piece[] pieces = new Piece[] { new Piece(1), new Piece(2), new Piece(3), new Piece(4) };

        public string color = "";

        Dice dice;

        int saved_roll = -1;

        public Player(Dice dice, string color)
        {
            this.dice = dice;
            this.color = color;
        }

        public Piece GetPiece(int id)
        {
            foreach (Piece p in pieces)
            {
                if (p.id == id)
                {
                    return p;
                }
            }
            return null;
        }

        public int RollDice() {
            if (saved_roll >= 1)
            {
                throw new Exception("You have already rolled the dice");
            }
            saved_roll = dice.Roll();
            return saved_roll;
        }

        public void ResetDiceRoll()
        {
            saved_roll = -1;
        }

        public void MovePieceOntoBoard(int choice)
        {
            if (saved_roll != 6)
            {
                throw new Exception("You have to roll a 6 to move a new piece onto the board");
            }
            pieces[choice].position = 1;
        }

        public void PlayerMovePiece(int id)
        {
            if (pieces[id].IsInYard() || pieces[id].IsInGoal())
            {
                throw new Exception("Can not move this piece. Please try again.");
            }
            if (pieces[id].position > 56 && pieces[id].StepsUntilGoal() != saved_roll)
            {
                throw new Exception("You can not move a piece due to the rules restrictions. You skip the move until you throw a certain dice number");
            }
            pieces[id].Move(saved_roll);
        }

        public string ShowInfo()
        {
            string info = "";
            foreach (Piece P in pieces)
            {
                if (P.IsInYard())
                {
                    info += "The Player's " + color +
                        " piece number " + P.id + " is in position: 0\n";
                }
                else
                {
                    info += "The Player's " + color +
                      " piece number " + P.id + " is in position: " + P.position +
                      " and has this many steps until the goal: " + P.StepsUntilGoal() + "\n";
                }
            }
            return info;
        }

        public bool CanPutPieceInPlay(int diceroll)
        {
            if (diceroll != 6)
            {
                return false;
            }
            else
            {
                foreach (Piece P in pieces)
                {
                    if (P.IsInYard())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanMoveAnyPieces(int diceroll)
        {
            foreach (Piece p in pieces)
            {
                if (!p.IsInYard() && !p.IsInGoal() && !(p.position > 56 && p.StepsUntilGoal() != diceroll))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Won()
        {
            foreach (Piece p in pieces)
            {
                if (!p.IsInGoal())
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Dice
    {
        Random rnd = new Random();

        public int Roll()
        {
            return rnd.Next(1, 7);
        }
    }

    public class Board
    {
        List<Player> players = new List<Player>();

        Dice dice = new Dice();

        int turn;

        bool started = false;

        bool game_complete = false;
        string winner = "";

        public Dice GetDice()
        {
            return dice;
        }

        public void DeclareWinner(string player_color)
        {
            winner = player_color;
            game_complete = true;
        }

        public string GetWinner()
        {
            return winner;
        }

        public bool IsGameOver()
        {
            return game_complete;
        }

        public void AddPlayer(string color)
        {
            Player P = new Player(dice, color);

            if (started)
            {
                throw new Exception("Can't add player to a running game");
            }

            foreach (Player other in players)
            {
                if (other.color == P.color)
                {
                    throw new Exception($"Can't add a player because {P.color} is taken!");
                }
            }

            if (players.Count >= 4)
            {
                throw new Exception("There is already four players in the game!");
            }

            players.Add(P);
        }

        public void StartGame()
        {
            if (players.Count < 2)
            {
                throw new Exception("Not enough players to start the game!");
            }
            started = true;
        }

        public bool IsGameStarted()
        {
            return started;
        }

        public Player GetCurrentPlayer()
        {
            return players[turn];
        }

        public void NextTurn()
        {
            GetCurrentPlayer().ResetDiceRoll();
            turn = turn + 1;
            if (turn >= players.Count)
            {
                turn = 0;
            }
        }
    }
}