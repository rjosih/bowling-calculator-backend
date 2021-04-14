using System.Collections.Generic;
using System.Linq;
public class ResponseModelMapper : IMapper<OutputModel, ResponseModel>
{
    public ResponseModel Map(OutputModel input)
    {
        var players = input.Players.Select(
        x => new PlayerResponseModel
        {
            PlayerID = x.PlayerID,
            Round = x.Round,
            ScoreWay = (APIScoreWay)x.ScoreWay,
            Score1 = x.Score1,
            Score2 = x.Score2,
            TotalSummary = x.TotalSummary

        }).ToList();
        return new ResponseModel
        {
            players = players
        };
    }
}