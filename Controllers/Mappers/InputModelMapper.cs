using System.Collections.Generic;
using System.Linq;
public class InputModelMapper : IMapper<RequestModel, InputModel>
{
    private readonly IMapper<PlayerRequestModel, Player> playerMapper;
    public InputModelMapper()
    {
        this.playerMapper = new PlayerMapper();
    }
    public InputModel Map(RequestModel input)
    {
        var players = input.players.Select(
        x => new Player
        {
            PlayerID = x.PlayerID,
            Round = x.Round,
            ScoreWay = (ScoreWay)x.ScoreWay,
            Score1 = x.Score1,
            Score2 = x.Score2,
            TotalSummary = x.TotalSummary

        }).ToList();
        return new InputModel
        {
            Players = players
        };
    }
}