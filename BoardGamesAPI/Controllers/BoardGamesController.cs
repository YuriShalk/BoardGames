using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace BoardGamesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class BoardGamesController : ControllerBase
{
    private readonly ILogger<BoardGamesController> _logger;

    public BoardGamesController(ILogger<BoardGamesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("api/boardGamesList")]
    public IActionResult Get([FromQuery] string name = "", [FromQuery] int year = 0)
    {
        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "board-games-ranking.json");
            string jsonContent = System.IO.File.ReadAllText(filePath);
            List<BoardGames> boardGamesList = JsonSerializer.Deserialize<List<BoardGames>>(jsonContent);

            if(!string.IsNullOrEmpty(name))
                boardGamesList = boardGamesList.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();

            if(year > 0)
                boardGamesList = boardGamesList.Where(x => x.Year == year).ToList();

            return Ok(boardGamesList);
        }
        catch (Exception)
        {
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

        return Ok(newBoardGame);
    }
}