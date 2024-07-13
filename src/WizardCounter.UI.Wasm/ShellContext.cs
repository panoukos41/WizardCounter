using System.Reactive.Subjects;

namespace WizardCounter;

public sealed class ShellContext
{
    private static readonly BehaviorSubject<string> titleSub = new(string.Empty);

    public static IObservable<string> TitleChanged { get; } = titleSub.AsObservable();

    public static string Title
    {
        get => titleSub.Value;
        set {
            if (titleSub.Value.Equals(value)) return;

            titleSub.OnNext(value);
        }
    }
}
