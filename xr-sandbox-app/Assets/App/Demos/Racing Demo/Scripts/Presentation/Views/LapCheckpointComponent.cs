using UnityEngine;
using App.Demos.RacingDemo.Scripts.Domain.UseCases;
using App.Demos.CarDemo.Scripts;

namespace App.Demos.RacingDemo.Scripts.Presentation.Views
{
    /// <summary>
    /// A dumb component that forwards collision events to the Use Case.
    /// </summary>
    public class LapCheckpointComponent : MonoBehaviour
    {
        [SerializeField] private bool isStartFinishLine = false;
        
        // DEPENDENCY INVERSION: Relies on Interface
        private IProcessCheckpointUseCase _checkpointUseCase;

        [VContainer.Inject]
        public void Construct(IProcessCheckpointUseCase checkpointUseCase)
        {
            _checkpointUseCase = checkpointUseCase;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_checkpointUseCase == null) return;

            if (other.GetComponentInParent<CarController>())
            {
                _checkpointUseCase.Execute(isStartFinishLine);
            }
        }
    }
}
