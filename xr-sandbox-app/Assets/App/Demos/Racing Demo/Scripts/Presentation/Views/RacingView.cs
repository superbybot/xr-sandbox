using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using App.Demos.RacingDemo.Scripts.Domain;
using App.Demos.RacingDemo.Scripts.Domain.UseCases;
using App.Demos.DialogueDemo.Scripts;
using App.Demos.CarDemo.Scripts;


namespace App.Demos.RacingDemo.Scripts.Presentation.Views
{
    /// <summary>
    /// The Humble View.
    /// Responsible for connecting Unity Events (Input) -> UseCases
    /// And connecting State Changes -> Visuals (Audio/UI)
    /// </summary>
    public class RacingView : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CarTeleportAnchor carAnchor;
        
        private RacingState _state;
        // DEPENDENCY INVERSION: Relies on Interface, not Concrete Class
        private IStartRaceUseCase _startRaceUseCase;

        [VContainer.Inject]
        public void Construct(RacingState state, IStartRaceUseCase startRaceUseCase)
        {
            _state = state;
            _startRaceUseCase = startRaceUseCase;

            BindState();
            BindInput();
        }

        private readonly CompositeDisposable _disposables = new();

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void BindInput()
        {
            // When player enters car -> Trigger Start Race Use Case
            if (carAnchor != null)
            {
                // Use the Exposed Observable from CarTeleportAnchor
                carAnchor.CarEnterObservable
                    .Subscribe(_ => _startRaceUseCase.ExecuteAsync().Forget())
                    .AddTo(_disposables);
            }
        }

        private void BindState()
        {
            _state.CurrentPhase
                .Subscribe(OnPhaseChanged)
                .AddTo(_disposables);

            _state.CurrentLap
                .Where(lap => lap > 1) // Don't show for lap 1 start which is implicit
                .Subscribe(lap => ShowLapMessage(lap))
                .AddTo(_disposables);
        }

        private void OnPhaseChanged(RacePhase phase)
        {
            switch (phase)
            {
                case RacePhase.Countdown:
                    PlayCountdownSequence().Forget();
                    break;
                    
                case RacePhase.Racing:
                    PromptManager.ShowPrompt("GO!!!", 1f);
                    break;

                case RacePhase.Finished:
                    PromptManager.ShowPrompt("🏆 RACE FINISHED! 🏆", 5f);
                    break;
            }
        }

        private void ShowLapMessage(int lap)
        {
             PromptManager.ShowPrompt($"Lap {lap}/{_state.TotalLaps}", 2f);
        }

        private async UniTaskVoid PlayCountdownSequence()
        {
            PromptManager.ShowPrompt("3", 1f);
            await UniTask.WaitForSeconds(1f);
            
            PromptManager.ShowPrompt("2", 1f);
            await UniTask.WaitForSeconds(1f);
            
            PromptManager.ShowPrompt("1", 1f);
            await UniTask.WaitForSeconds(1f);
        }
    }
}
