using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ludo.Engine;

namespace LudoAPI
{
    // NOTE: This is an interface for the GameHandler. It should contain all method definitions used
    // by the controllers. The controllers only knows about the methods defined here.
    public interface IGameHandler
    {
        Guid CreateGame();

        Board GetBoard(Guid game_id);

        string ListGames();

    }

   
}
