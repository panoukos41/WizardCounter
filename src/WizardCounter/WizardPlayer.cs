using Core;
using Core.Abstractions;
using FluentValidation;
using System.Text.Json.Serialization;

namespace WizardCounter;

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
