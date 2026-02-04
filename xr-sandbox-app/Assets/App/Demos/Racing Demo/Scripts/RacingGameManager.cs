using System;
using UnityEngine;
using App.Shared.Scripts.Meta; // MetaCarTeleportAnchor
using App.Demos.CarDemo.Scripts;
using App.Demos.DialogueDemo.Scripts;
using Cysharp.Threading.Tasks;

namespace App.Demos.RacingDemo.Scripts
{
    public class RacingGameManager : MonoBehaviour
    {
        // Configuration
        [SerializeField] private int totalLaps = 3;
        [SerializeField] private MetaCarTeleportAnchor carTeleportAnchor;
        [SerializeField] private CarInputManager carInputManager;
        [SerializeField] private RaceCountdown raceCountdown;
        
        // State
        private RaceState currentState = RaceState.WaitingForPlayer;
        private int currentLap = 0;
        
        public enum RaceState
        {
            WaitingForPlayer,   // Player standing, car waiting
            Countdown,          // 3, 2, 1, GO!
            Racing,             // Active racing
            Finished            // Race complete
        }
        
        // Events
        public event Action<int> OnLapCompleted;
        public event Action OnRaceFinished;

        private void OnEnable()
        {
            if (carTeleportAnchor != null)
            {
                carTeleportAnchor.OnCarEnter.AddListener(HandleCarEnter);
            }
        }

        private void OnDisable()
        {
            if (carTeleportAnchor != null)
            {
                carTeleportAnchor.OnCarEnter.RemoveListener(HandleCarEnter);
            }
        }

        private void Start()
        {
            // Ensure input is disabled initially if implied, but actually 
            // CarInputManager handles its own state. 
            // However, we want to prevent driving during countdown.
            // If player enters car, CarInputManager usually enables input. 
            // We will need to disable it during countdown.
            
            // If we are waiting for player, we don't do much.
        }

        private void HandleCarEnter()
        {
            if (currentState == RaceState.WaitingForPlayer)
            {
                StartRaceSequence().Forget();
            }
        }

        private async UniTaskVoid StartRaceSequence()
        {
            // Transition to Countdown
            currentState = RaceState.Countdown;
            
            // Disable car input
            if (carInputManager != null)
            {
                carInputManager.enabled = false;
            }
            
            // Start Countdown
            if (raceCountdown != null)
            {
                await raceCountdown.StartCountdownAsync();
            }
            else
            {
                // Fallback if no countdown component
                await UniTask.WaitForSeconds(3f);
            }
            
            // Start Racing
            StartRacing();
        }

        private void StartRacing()
        {
            currentState = RaceState.Racing;
            currentLap = 0; // Or 1? Usually 0 completed laps.
            
            // Enable car input
            if (carInputManager != null)
            {
                carInputManager.enabled = true;
            }
            
            Debug.Log("Race Started!");
        }

        public void OnCheckpointPassed(LapCheckpoint checkpoint)
        {
            if (currentState != RaceState.Racing) return;

            if (checkpoint.IsStartFinishLine)
            {
                // Simple lap counting: if we hit start/finish, increment lap.
                // In a real game, we'd check if we passed all checkpoints.
                // For this demo, just looping is enough.
                
                currentLap++;
                OnLapCompleted?.Invoke(currentLap);
                
                // Show lap prompt?
                PromptManager.ShowPrompt($"Lap {currentLap}/{totalLaps}", 2f);

                if (currentLap >= totalLaps)
                {
                    FinishRace();
                }
            }
        }

        private void FinishRace()
        {
            currentState = RaceState.Finished;
            OnRaceFinished?.Invoke();
            
            // Disable input or let them drive? 
            // Usually let them drive a cooldown lap, but maybe show message.
            PromptManager.ShowPrompt("🎉 Race Complete! 🎉", 5f);
        }
    }
}
