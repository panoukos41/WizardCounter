using Core;
using Core.Abstractions;
using FluentValidation;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace WizardCounter;

public enum WizardGameState
{
    Initializing,
    Initialized,
    Preparing,
    Playing,
    Counting,
    Finished
}

public sealed record class WizardGame : IValid<WizardGame>
{
    public WizardGame()
    {
        Id = Uuid.NewUuid();
    }

    public WizardGame(Uuid id)
    {
        Id = id;
    }

    public Uuid Id { get; }

    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WizardGameState State { get; set; }

    public WizardPlayerCollection Players { get; set; } = [];

    public List<WizardGameRound> Rounds { get; set; } = [];

    public static IValidator<WizardGame> Validator { get; } = InlineValidator.For<WizardGame>(data =>
    {
        data.RuleFor(x => x.Name)
            .Length(1, 50);

        data.RuleFor(x => x.Players).Must(x => x.Count is > 1)
            .WithMessage("You need two players and more.");

        data.RuleForEach(x => x.Players)
            .SetValidator(WizardPlayer.Validator);
    });
}

public sealed record WizardPlayer : IValid<WizardPlayer>
{
    public WizardPlayer()
    {
        Id = Uuid.NewUuid();
    }

    [JsonConstructor]
    public WizardPlayer(Uuid id)
    {
        Id = id;
    }

    public Uuid Id { get; }

    public string Name { get; set; } = string.Empty;

    public static IValidator<WizardPlayer> Validator { get; } = InlineValidator.For<WizardPlayer>(data =>
    {
        data.RuleFor(x => x.Name).Length(1, 50);
    });
}

public sealed class WizardPlayerCollection : KeyedCollection<Uuid, WizardPlayer>
{
    public WizardPlayerCollection()
    {
    }

    [JsonConstructor]
    public WizardPlayerCollection(IEnumerable<WizardPlayer> players)
    {
        foreach (var player in players)
        {
            Add(player);
        }
    }

    protected override Uuid GetKeyForItem(WizardPlayer item)
    {
        return item.Id;
    }
}
