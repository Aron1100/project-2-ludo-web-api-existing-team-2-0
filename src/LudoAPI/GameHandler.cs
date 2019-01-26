using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ludo.Engine;

namespace LudoAPI
{
    // NOTE: This implements the methods defined in the IGameHandler.
    public class GameHandler : IGameHandler
    {
        Dictionary<Guid, Board> games = new Dictionary<Guid, Board>();

        public Guid CreateGame()
        {
            Guid guid = Guid.NewGuid();
            Board board = new Board();
            games.Add(guid, board);
            return guid;
        }

        public Board GetBoard(Guid game_id)
        {
            return games[game_id];
        }

        public string ListGames()
        {
            return string.Join('\n', games.Keys);
        }
    }

}
