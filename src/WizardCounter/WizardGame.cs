using Core;
using Core.Abstractions;
using FluentValidation;
using System.Text.Json.Serialization;

namespace WizardCounter;

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

    public WizardRoundCollection Rounds { get; set; } = [];

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
