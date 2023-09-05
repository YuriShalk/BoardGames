using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BoardGamesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class BoardGamesController : ControllerBase
{
    private readonly ILogger<BoardGamesController> _logger;
    private readonly IMemoryCache _memoryCache;

    public BoardGamesController(ILogger<BoardGamesController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    [Route("api/boardGamesList")]
    public IActionResult Get([FromQuery] string name = "", [FromQuery] int year = 0)
    {
        try
        {
            if(!_memoryCache.TryGetValue("boardGamesList", out List<BoardGames> boardGamesList))
            {
                return StatusCode(500, "Dados não encontrados");
            }

            List<BoardGames> filteredGames = boardGamesList;

            if(!string.IsNullOrEmpty(name))
                boardGamesList = boardGamesList.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();

            if(year > 0)
                boardGamesList = boardGamesList.Where(x => x.Year == year).ToList();

            var response = new BoardGamesGetResponse
            {
                Message = $"Foram encontrados {boardGamesList.Count} jogos",
                Games = boardGamesList,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no método GET: {ex.Message}");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Route("api/addBoardGames")]
    public IActionResult Post(BoardGames boardGame)
    {
        if (boardGame == null)
        {
            return BadRequest("Dados Inválidos");
        }

        var newBoardGame = new BoardGames
        {
            Name = boardGame.Name,
            Category = boardGame.Category,
            Publisher = boardGame.Publisher,
            Year = boardGame.Year
        };

        // add the new board game to the list and save it to the cache
        List<BoardGames> boardGamesList;
        if (!_memoryCache.TryGetValue("boardGamesList", out boardGamesList))
        {
            boardGamesList = new List<BoardGames>();
        }
        boardGamesList.Add(newBoardGame);
        _memoryCache.Set("boardGamesList", boardGamesList);

        return Ok(newBoardGame);
    }
}