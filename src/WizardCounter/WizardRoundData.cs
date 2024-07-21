namespace WizardCounter;

public sealed record WizardRoundData
{
    public int Bets { get; set; }

    public int Wins { get; set; }

    public int CalculatePoints()
    {
        if (Wins == Bets)
        {
            return 20 + (Bets * 10);
        }
        return Math.Abs(Bets - Wins) * -10;
    }
}
