using Core;
using Core.Abstractions;
using Core.Reactive;
using FluentValidation.Results;
using Stateless;

namespace WizardCounter;

public sealed class WizardGameViewModel : RxObject
{
    private readonly WizardGame game;
    private readonly StateMachine<WizardGameState, WizardGameState> stateMachine;

    public string Name => game.Name;

    public WizardGameState State
    {
        get => game.State;
        private set => game.State = value;
    }

    public int MaxRounds => (int)Math.Floor(54f / game.Players.Count);

    public bool IsFinished => State is WizardGameState.Finished || game.Rounds.Count == MaxRounds;

    public int CardsPerPlayer => CurrentRound;

    public int CurrentRound => game.Rounds.Count;

    public WizardRound ActiveRound => game.Rounds[^1];

    public WizardGameViewModel() : this(new())
    {
    }

    public WizardGameViewModel(WizardGame game)
    {
        this.game = game;
        stateMachine = new(game.State);

        stateMachine.OnTransitionCompleted(state =>
        {
            game.State = state.Destination;
            RaisePropertyChanged();
        });

        stateMachine.Configure(WizardGameState.Initializing)
            .Permit(WizardGameState.Preparing, WizardGameState.Preparing);

        stateMachine.Configure(WizardGameState.Preparing)
            .Permit(WizardGameState.Betting, WizardGameState.Betting);

        stateMachine.Configure(WizardGameState.Betting)
            .Permit(WizardGameState.Playing, WizardGameState.Playing)
            .OnEntry(() => game.Rounds.Add(new(game.Players.Select(p => p.Id))));

        stateMachine.Configure(WizardGameState.Playing)
            .Permit(WizardGameState.Counting, WizardGameState.Counting);

        stateMachine.Configure(WizardGameState.Counting)
            .Permit(WizardGameState.Preparing, WizardGameState.Preparing)
            .Permit(WizardGameState.Finished, WizardGameState.Finished);

        stateMachine.Configure(WizardGameState.Finished);
    }

    public Result<Nothing> Next()
    {
        return State switch
        {
            WizardGameState.Initializing => Prepare(),
            WizardGameState.Preparing => Bet(),
            WizardGameState.Betting => Play(),
            WizardGameState.Playing => Count(),
            WizardGameState.Counting => Prepare(),
            //WizardGameState.Finished =>
            _ => Problem.Create("Can't transition a finished game")
        };
    }

    public Result<Nothing> Prepare()
    {
        if (CurrentRound == MaxRounds)
            return TransitionTo(WizardGameState.Finished);

        return TransitionTo(WizardGameState.Preparing);
    }

    public Result<Nothing> Bet()
    {
        return TransitionTo(WizardGameState.Betting);
    }

    public Result<Nothing> Play()
    {
        return TransitionTo(WizardGameState.Playing);
    }

    public Result<Nothing> Count()
    {
        return TransitionTo(WizardGameState.Counting);
    }

    public Result<Nothing> Finish()
    {
        return TransitionTo(WizardGameState.Finished);
    }

    public string GetPlayerName(Uuid playerId)
    {
        return game.Players[playerId].Name;
    }

    public Result<WizardPlayer> AddPlayer(WizardPlayer? player = null)
    {
        if (State is not WizardGameState.Initializing) return new Problem
        {
            Title = "Invalid Operation",
            Type = "InvalidOperation",
            Detail = "Can't add player in an already initialized game."
        };

        player ??= new WizardPlayer();
        game.Players.Add(player);
        RaisePropertyChanged();
        return player;
    }

    public Result<None> RemovePlayer(Uuid id)
    {
        if (State is not WizardGameState.Initializing) return new Problem
        {
            Title = "Invalid Operation",
            Type = "InvalidOperation",
            Detail = "Can't add player in an already initialized game."
        };

        game.Players.Remove(id);
        RaisePropertyChanged();
        return None.Value;
    }

    public IEnumerable<WizardPlayer> Players => game.Players.AsEnumerable();

    public IEnumerable<(Uuid PlayerId, int Points)> CalculateScore()
    {
        return game.Rounds
            .SelectMany(round => round)
            .GroupBy(round => round.Key, round => round.Value) // Key is Player
            .Select(playerGroup => (PlayerId: playerGroup.Key, Points: playerGroup.Sum(x => x.CalculatePoints())))
            .OrderByDescending(result => result.Points);
    }

    public ValidationResult Validate()
    {
        return game.Validate();
    }

    private Result<Nothing> TransitionTo(WizardGameState state)
    {
        if (stateMachine.CanFire(state))
        {
            stateMachine.Fire(state);
            return Nothing.Value;
        }
        return Problem.Create($"Can't transition to {state} from {State}");
    }
}
