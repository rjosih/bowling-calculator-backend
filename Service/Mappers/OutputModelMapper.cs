using System.Collections.Generic;
using System.Linq;
public class OutputModelMapper : IMapper<InputModel, OutputModel>
{
    public OutputModel Map(InputModel input)
    {
        var players = input.Players.Select(
        x => new Player
        {
            PlayerID = x.PlayerID,
            Round = x.Round,
            ScoreWay = x.ScoreWay,
            Score1 = x.Score1,
            Score2 = x.Score2,
            TotalSummary = x.TotalSummary

        }).ToList();
        return new OutputModel
        {
            Players = players
        };
    }
}