namespace WizardCounter.Components;

[Flags]
public enum Anchor
{
    Bottom = 1 << 0,
    Top = 1 << 1,
    Start = 1 << 2,
    End = 1 << 3,
    Center = 1 << 4
}
