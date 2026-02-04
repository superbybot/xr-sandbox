using System;
using UnityEngine;
using App.Demos.CarDemo.Scripts;

namespace App.Demos.RacingDemo.Scripts
{
    public class LapCheckpoint : MonoBehaviour
    {
        [SerializeField] private bool isStartFinishLine = true;
        [SerializeField] private int checkpointIndex = 0;
        [SerializeField] private RacingGameManager gameManager;
        
        public bool IsStartFinishLine => isStartFinishLine;
        public int CheckpointIndex => checkpointIndex;
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if it's the player car
            // We can look for CarController or any identifying component
            // CarController is on the root or near it usually.
            // The collider might be a wheel or body part.
            
            var car = other.GetComponentInParent<CarController>();
            if (car != null)
            {
                if (gameManager != null)
                {
                    gameManager.OnCheckpointPassed(this);
                }
            }
        }
    }
}
