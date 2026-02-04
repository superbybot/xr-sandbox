using R3;

namespace App.Demos.RacingDemo.Scripts.Domain
{
    /// <summary>
    /// Pure Domain Entity holding the state of the race.
    /// has no logic, just data.
    /// </summary>
    public class RacingState
    {
        // Reactive Properties imply "The value changed" event built-in.
        public ReactiveProperty<RacePhase> CurrentPhase { get; } = new(RacePhase.Waiting);
        public ReactiveProperty<int> CurrentLap { get; } = new(0);
        public int TotalLaps { get; }

        public RacingState(int totalLaps)
        {
            TotalLaps = totalLaps;
        }
    }

    public enum RacePhase
    {
        Waiting,
        Countdown,
        Racing,
        Finished
    }
}
