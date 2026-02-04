namespace App.Demos.RacingDemo.Scripts.Domain.UseCases
{
    /// <summary>
    /// Strict Interface for Processing Checkpoints.
    /// This is the "Input Port" that the View talks to.
    /// </summary>
    public interface IProcessCheckpointUseCase
    {
        void Execute(bool isFinishLine);
    }

    public class ProcessCheckpointUseCase : IProcessCheckpointUseCase
    {
        private readonly RacingState _state;

        public ProcessCheckpointUseCase(RacingState state)
        {
            _state = state;
        }

        public void Execute(bool isFinishLine)
        {
            if (_state.CurrentPhase.Value != RacePhase.Racing) 
                return;

            if (isFinishLine)
            {
                IncrementLap();
            }
        }

        private void IncrementLap()
        {
            if (_state.CurrentLap.Value >= _state.TotalLaps)
            {
                FinishRace();
            }
            else
            {
                _state.CurrentLap.Value++;
            }
        }

        private void FinishRace()
        {
            _state.CurrentPhase.Value = RacePhase.Finished;
        }
    }
}
