namespace BoardGamesAPI.Controllers;

public class BoardGamesGetResponse
{
    public string Message { get; set; }
    public List<BoardGames> Games { get; set; }
    
}