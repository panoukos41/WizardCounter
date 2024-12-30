using Core.Preferences.Builders;
using Core.Preferences.Controls;

namespace WizardCounter;

public static class Preferences
{
    public const string Language = "lang";

    public const string Theme = "theme";

    public const string DeckSize = "default-deck-size";

    public static PreferenceScreen Screen { get; } = PreferenceScreenBuilder
        .CreateEmpty()
        .AddCategory(b => b
            .WithTitle("settings.app.title")
            .AddListBox(new()
            {
                Key = Preferences.Theme,
                Title = "settings.theme.title",
                AllowedValues = ["auto", "dark", "light"],
                DefaultValue = "auto",
                SummaryProvider = value => $"settings.theme.{value}",
            })
            .AddListBox(new()
            {
                Key = Preferences.Language,
                Title = "settings.lang.title",
                AllowedValues = ["en", "el"],
                DefaultValue = "en",
                SummaryProvider = value => $"settings.lang.{value}",
            })
        )
        .AddCategory(b => b
            .WithTitle("settings.game.title")
            .AddEditNumber(new EditNumberPreferenceBuilder<int>()
            {
                Key = Preferences.DeckSize,
                Title = "default-deck-size",
                DefaultValue = 54,
                Step = 1,
                Min = 54,
                Max = 120
            })
        )
    .Build();
}
