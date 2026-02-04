using Cysharp.Threading.Tasks;

namespace App.Demos.RacingDemo.Scripts.Domain.UseCases
{
    /// <summary>
    /// Strict Interface for Starting the Race.
    /// This is the "Input Port" that the View talks to.
    /// </summary>
    public interface IStartRaceUseCase
    {
        UniTask ExecuteAsync();
    }

    public class StartRaceUseCase : IStartRaceUseCase
    {
        private readonly RacingState _state;

        public StartRaceUseCase(RacingState state)
        {
            _state = state;
        }

        public async UniTask ExecuteAsync()
        {
            if (_state.CurrentPhase.Value != RacePhase.Waiting) 
                return;

            _state.CurrentPhase.Value = RacePhase.Countdown;

            await UniTask.WaitForSeconds(3f);

            _state.CurrentPhase.Value = RacePhase.Racing;
            _state.CurrentLap.Value = 1;
        }
    }
}
