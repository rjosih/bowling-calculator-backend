using System.Collections.Generic;

public class RequestModel 
{
    public List<PlayerRequestModel> players {get; set; }
    public int lastScore { get; set; }
    public int rolls { get; set; }
    public List<int[]> pins { get; set; }
    public List<int[]> frames { get; set; }
    public int[] cumulativeScores { get; set; }
}