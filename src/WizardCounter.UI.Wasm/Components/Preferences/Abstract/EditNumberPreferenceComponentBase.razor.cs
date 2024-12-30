using System.Numerics;

namespace WizardCounter.Components.Preferences.Abstract;

public abstract partial class EditNumberPreferenceComponentBase<TNumber> where TNumber : notnull, INumber<TNumber>, new()
{
}
