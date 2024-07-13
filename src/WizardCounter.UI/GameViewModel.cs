using Core;
using Core.Abstractions;
using Core.Reactive;
using FluentValidation.Results;

namespace WizardCounter;

public sealed class GameViewModel : RxObject
{
    private WizardGame Game { get; }

    public string Name => Game.Name;

    public WizardGameState State
    {
        get => Game.State;
        private set => Game.State = value;
    }

    public int MaxRounds => (int)Math.Floor(54f / Game.Players.Count);

    public bool IsFinished => State is WizardGameState.Finished || Game.Rounds.Count == MaxRounds;

    public int CardsPerPlayer => CurrentRound;

    public int CurrentRound => Game.Rounds.Count;

    public WizardGameRound ActiveRound => Game.Rounds[^1];

    public GameViewModel()
    {
        Game = new();
    }

    public GameViewModel(WizardGame game)
    {
        Game = game;
    }

    public Result<None> Prepare()
    {
        if (State is not (WizardGameState.Counting or WizardGameState.Initialized or WizardGameState.Initializing))
        {
            return WizardProblems.InvalidState;
        }
        if (IsFinished)
        {
            State = WizardGameState.Finished;
            RaisePropertyChanged();
            return None.Value;
        }

        Game.Rounds.Add(new(Game.Players.Select(p => p.Id)));
        State = WizardGameState.Preparing;
        RaisePropertyChanged();
        return None.Value;
    }

    public Result<None> Play()
    {
        if (State is not WizardGameState.Preparing)
        {
            return WizardProblems.InvalidState;
        }

        State = WizardGameState.Playing;
        RaisePropertyChanged();
        return None.Value;
    }

    public Result<None> Count()
    {
        if (State is not WizardGameState.Playing)
        {
            return WizardProblems.InvalidState;
        }

        State = WizardGameState.Counting;
        RaisePropertyChanged();
        return None.Value;
    }


    public string GetPlayerName(Uuid playerId)
    {
        return Game.Players[playerId].Name;
    }

    public void AddPlayer()
    {
        if (State is not WizardGameState.Initializing) return;

        Game.Players.Add(new WizardPlayer());
        RaisePropertyChanged();
    }

    public void RemovePlayer(Uuid id)
    {
        if (State is not WizardGameState.Initializing) return;

        Game.Players.Remove(id);
        RaisePropertyChanged();
    }

    public IEnumerable<WizardPlayer> Players => Game.Players.AsEnumerable();

    public IEnumerable<(Uuid PlayerId, int Points)> CalculateScore()
    {
        return Game.Rounds
            .SelectMany(round => round)
            .GroupBy(round => round.Key, round => round.Value) // Key is Player
            .Select(playerGroup => (PlayerId: playerGroup.Key, Points: playerGroup.Sum(x => x.GetPoints())))
            .OrderByDescending(result => result.Points);
    }

    public ValidationResult Validate()
    {
        return Game.Validate();
    }
}
