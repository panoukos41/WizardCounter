using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardCounter;

public static class WizardProblems
{
    public static Problem Finished { get; } = new()
    {
        Title = "Error",
        Type = nameof(Finished),
    };

    public static Problem InvalidState { get; } = new()
    {
        Title = "Invalid State",
        Type = nameof(InvalidState),
    };

    public static Problem InvalidWins { get; } = new()
    {
        Title = "Invalid Wins",
        Type = nameof(InvalidWins),
    };
}
