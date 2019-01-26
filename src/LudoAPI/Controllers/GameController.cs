using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ludo.Engine;

namespace LudoAPI.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        IGameHandler gameHandler;

        // NOTE: The GameHandler is a singleton (there is only one instance of it in the application), and it's
        // given to us via .NET-magic (see services.AddSingleton in Startup.cs).
        public GameController(IGameHandler gh) {
            gameHandler = gh;
        }

        public IActionResult GameOver(Board board)
        {
            return Ok($"Game is over. {board.GetWinner()} won");
        }

        // GET: api/game/listgames
        [HttpGet("listgames")]
        public IActionResult ListGames()
        {
            return Ok(gameHandler.ListGames());
        }

        // POST: api/game/create
        [HttpPost("create")]
        public IActionResult CreateNewGame()
        {
            Guid id = gameHandler.CreateGame();
            return Ok(id.ToString());
        }

        [HttpPost("{game_id}/start")]
        public IActionResult StartGame(Guid game_id)
        {
            Board board = gameHandler.GetBoard(game_id);
            try
            {
                board.StartGame();
                return Ok("Started the game! No new players can be added.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/game/{game_id}/add_player
        [HttpPost("{game_id}/add_player")]
        public IActionResult AddPlayer(Guid game_id, string color)
        {
            // NOTE: Use gameHandler to get och update the board. The user needs to send
            // a board ID with each web request so we know which game she's playing.
            Board board = gameHandler.GetBoard(game_id);
            try
            {
                board.AddPlayer(color);
                return Ok($"Added player {color} to board {game_id}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // NOTE: This is to see who's turn it is.
        // GET: api/game/{game_id}/current_player"
        [HttpGet("{game_id}/current_player")]
        public IActionResult CurrentPlayer(Guid game_id)
        {
            Board board = gameHandler.GetBoard(game_id);
            return Ok(board.GetCurrentPlayer().color);
        }

        [HttpGet("{game_id}/player/info")]
        public IActionResult ShowPlayerInfo(Guid game_id, string color)
        {
            Board board = gameHandler.GetBoard(game_id);
            Player player = board.GetCurrentPlayer();

            return Ok(player.ShowInfo());
        }

        [HttpGet("{game_id}/player/roll_dice")]
        public IActionResult RequestDiceRoll(Guid game_id, string color)
        {
            Board board = gameHandler.GetBoard(game_id);
            if (board.IsGameOver())
            {
                return GameOver(board);
            }

            // NOTE: It might not be the requesting player's turn to roll the dice.
            Player player = board.GetCurrentPlayer();
            if (player.color != color)
            {
                return BadRequest("It's not your turn!");
            }

            try
            {
                int number = player.RollDice();

                // Check if the player can make any move. If she can't, advance the game.
                if (!player.CanMoveAnyPieces(number) && !player.CanPutPieceInPlay(number))
                {
                    board.NextTurn();
                    return Ok("You can't make any moves.");
                }

                return Ok($"You rolled {number}, make a move.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{game_id}/player/move")]
        public IActionResult PlayerMove(Guid game_id, string color, int piece, int steps)
        {
            Board board = gameHandler.GetBoard(game_id);
            if (board.IsGameOver())
            {
                return GameOver(board);
            }

            Player player = board.GetCurrentPlayer();

            Piece selectedPiece = player.GetPiece(piece);

            try
            {
                if (selectedPiece.IsInYard() && steps == 1)
                {
                    player.MovePieceOntoBoard(piece);
                }
                else
                {
                    player.PlayerMovePiece(piece);
                }
                if (player.Won())
                {
                    board.DeclareWinner(color);
                    return Ok("YOU HAVE WON THE GAME!");
                }
                else
                {
                    board.NextTurn();
                }
                return Ok("OK!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
    
}   
   



