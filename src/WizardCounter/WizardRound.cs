using Core;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WizardCounter;

/// <summary>
/// A wizard round. Consists of PlayerId (key) - RoundData (value) pairs.
/// </summary>
[JsonConverter(typeof(WizardRoundJsonConverter))]
public sealed class WizardRound : ReadOnlyDictionary<Uuid, WizardRoundData>
{
    /// <summary>
    /// Initialize a new instance of <see cref="WizardRound"/>.
    /// </summary>
    /// <param name="playerData"></param>
    public WizardRound(IDictionary<Uuid, WizardRoundData> playerData) : base(playerData)
    {
    }

    /// <summary>
    /// Initialize new instance of <see cref="WizardRound"/> for the provided players with default data.
    /// </summary>
    /// <param name="players">The players to initialize data for.</param>
    public WizardRound(IEnumerable<Uuid> players) : base(players.ToDictionary(x => x, _ => new WizardRoundData()))
    {
    }

    private sealed class WizardRoundJsonConverter : JsonConverter<WizardRound>
    {
        public override WizardRound? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new(JsonSerializer.Deserialize<IDictionary<Uuid, WizardRoundData>>(ref reader, options)!);
        }

        public override void Write(Utf8JsonWriter writer, WizardRound value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<ReadOnlyDictionary<Uuid, WizardRoundData>>(writer, value, options);
        }
    }
}
