using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class Service : IService
{
    private ICalculate calculate;
    private readonly ILogger _logger;
    private readonly IMapper<InputModel, OutputModel> outputModelMapper;
    public Service(ILogger logger)
    {
        this.calculate = new Calculate();
        this._logger = logger;
        this.outputModelMapper = new OutputModelMapper();
    }

    OutputModel IService.CalculateScore(InputModel input)
    {
        var players = new List<Player> ();
        
        for(var i = 0; i < input.Players.Count; i++)
        {
            var player = input.Players[i];
            if((ScoreWay)player.ScoreWay == ScoreWay.Spare)
            {
                
            }
            var scoreForOneRound = (player.Score1 + player.Score2);
            var totalSum = this.calculate.CalculateScore(player.TotalSummary, scoreForOneRound);
            player.Round ++;
            players.Add(player);
        }
        return new OutputModel 
        {
            Players = players
        };
    }
}