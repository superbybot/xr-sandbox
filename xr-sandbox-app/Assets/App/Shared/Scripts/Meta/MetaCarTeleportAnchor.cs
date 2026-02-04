using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using R3;

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Meta SDK version of CarTeleportAnchor.
    /// Handles parenting the OVRCameraRig to the car seat and disabling OVR locomotion.
    /// Triggered via an Interaction (e.g. Button Press or EventWrapper).
    /// </summary>
    public class MetaCarTeleportAnchor : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The car seat or transform to parent the player to.")]
        [SerializeField] private Transform seatPoint;
        
        [Tooltip("The main OVRCameraRig (Player Root).")]
        [SerializeField] private Transform ovrCameraRig;
        
        [Tooltip("Exit point transform (where to dump player on exit).")]
        [SerializeField] private Transform exitPoint;

        [Header("Events")]
        public UnityEvent OnCarEnter;
        public UnityEvent OnCarExit;

        // R3 Observables for Racing Demo Compatibility
        private readonly Subject<Unit> _onCarEnterSubject = new();
        private readonly Subject<Unit> _onCarExitSubject = new();
        public Observable<Unit> CarEnterObservable => _onCarEnterSubject;
        public Observable<Unit> CarExitObservable => _onCarExitSubject;

        private bool _isSeated = false;
        
        private void OnDestroy()
        {
            _onCarEnterSubject.Dispose();
            _onCarExitSubject.Dispose();
        }

        // Called via Meta Interaction Event (e.g. PointableUnityEventWrapper.WhenSelect)
        public void EnterCar()
        {
            if (_isSeated) return;
            EnterCarAsync().Forget();
        }

        public void ExitCar()
        {
            if (!_isSeated) return;
            ExitCarAsync().Forget();
        }

        private async UniTaskVoid EnterCarAsync()
        {
            _isSeated = true;

            // Optional: Disable Locomotion here if you have a reference to OVRPlayerController
            // var loc = ovrCameraRig.GetComponent<OVRPlayerController>();
            // if (loc) loc.EnableLinearMovement = false;

            if (ovrCameraRig != null && seatPoint != null)
            {
                ovrCameraRig.SetParent(seatPoint);
                ovrCameraRig.localPosition = Vector3.zero;
                ovrCameraRig.localRotation = Quaternion.identity;
            }

            OnCarEnter?.Invoke();
            _onCarEnterSubject.OnNext(Unit.Default);
            
            await UniTask.CompletedTask;
        }

        private async UniTaskVoid ExitCarAsync()
        {
            _isSeated = false;

            // Detach
            if (ovrCameraRig != null)
            {
                ovrCameraRig.SetParent(null);
                
                if (exitPoint != null)
                {
                    ovrCameraRig.position = exitPoint.position;
                    
                    // Align up vector
                    Vector3 forward = exitPoint.forward;
                    forward.y = 0; 
                    if (forward != Vector3.zero)
                        ovrCameraRig.rotation = Quaternion.LookRotation(forward, Vector3.up);
                }
            }
            
            // Re-enable Locomotion
            // var loc = ovrCameraRig.GetComponent<OVRPlayerController>();
            // if (loc) loc.EnableLinearMovement = true;

            OnCarExit?.Invoke();
            _onCarExitSubject.OnNext(Unit.Default);
            
            await UniTask.CompletedTask;
        }
    }
}
