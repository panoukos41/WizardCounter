using Core;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WizardCounter;

[JsonConverter(typeof(WizardGameRoundJsonConverter))]
public sealed class WizardGameRound : ReadOnlyDictionary<Uuid, WizardGameRoundData>
{
    [JsonConstructor]
    public WizardGameRound(IDictionary<Uuid, WizardGameRoundData> playerData) : base(playerData)
    {
    }

    public WizardGameRound(IEnumerable<Uuid> players) : base(players.ToDictionary(x => x, _ => new WizardGameRoundData()))
    {
    }

    public void UpdateBets(IEnumerable<KeyValuePair<Uuid, int>> playerBets)
    {
        foreach (var (player, bets) in playerBets)
        {
            if (TryGetValue(player, out var data))
                data.Bets = bets;
        }
    }

    public void UpdateWins(IEnumerable<KeyValuePair<Uuid, int>> playerWins)
    {
        foreach (var (player, wins) in playerWins)
        {
            if (TryGetValue(player, out var data))
                data.Wins = wins;
        }
    }

    public bool HasValidWins(int round)
    {
        return Values.Select(x => x.Wins).Sum() == round;
    }

    public IEnumerable<KeyValuePair<Uuid, int>> CalculatePlayerPoints()
    {
        return this.Select(x => new KeyValuePair<Uuid, int>(x.Key, x.Value.GetPoints()));
    }
}

public sealed record WizardGameRoundData
{
    public int Bets { get; set; }

    public int Wins { get; set; }

    public int GetPoints()
    {
        if (Wins == Bets)
        {
            return 20 + (Bets * 10);
        }
        return Math.Abs(Bets - Wins) * -10;
    }
}

public sealed class WizardGameRoundJsonConverter : JsonConverter<WizardGameRound>
{
    public override WizardGameRound? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<IDictionary<Uuid, WizardGameRoundData>>(ref reader, options)!);
    }

    public override void Write(Utf8JsonWriter writer, WizardGameRound value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize<ReadOnlyDictionary<Uuid, WizardGameRoundData>>(writer, value, options);
    }
}
