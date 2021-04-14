public class Calculate : ICalculate
{
    
    int ICalculate.CalculateScore(int totalScore, int scoreForOneRound)
    {
     return totalScore + scoreForOneRound;
    }

    int ICalculate.AddRound(int round)
    {
        return round ++;
    }
}