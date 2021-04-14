public class PlayerMapper : IMapper<PlayerRequestModel, Player>
{
    public Player Map(PlayerRequestModel input)
    {
        return new Player
        {
            PlayerID = input.PlayerID,
            Round = input.Round,
            ScoreWay = (ScoreWay)input.ScoreWay,
            Score1 = input.Score1,
            Score2 = input.Score2,
            TotalSummary = input.TotalSummary

        };
    }
}