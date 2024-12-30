using Core.Preferences.Abstract;

namespace WizardCounter.Components.Preferences.Abstract;

public abstract class PreferenceValueComponentBase<TPreference> : PreferenceComponentBase<TPreference>
    where TPreference : PreferenceValueBase
{
    protected Uuid Id { get; } = Uuid.NewUuid();

    protected string GetSummary(string value)
        => Preference.SummaryProvider(value);

    protected string GetValueSummary()
        => Preference.ValueSummary;
}

public abstract class PreferenceValueComponentBase<TPreference, TValue> : PreferenceValueComponentBase<TPreference>
    where TPreference : PreferenceValueBase<TValue>
    where TValue : notnull, IParsable<TValue>, new()
{
}
