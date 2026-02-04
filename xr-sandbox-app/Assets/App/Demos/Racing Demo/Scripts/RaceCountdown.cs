using UnityEngine;
using Cysharp.Threading.Tasks;
using App.Demos.DialogueDemo.Scripts;

namespace App.Demos.RacingDemo.Scripts
{
    public class RaceCountdown : MonoBehaviour
    {
        public async UniTask StartCountdownAsync()
        {
            PromptManager.ShowPrompt("3", 1f);
            await UniTask.WaitForSeconds(1f);
            
            PromptManager.ShowPrompt("2", 1f);
            await UniTask.WaitForSeconds(1f);
            
            PromptManager.ShowPrompt("1", 1f);
            await UniTask.WaitForSeconds(1f);
            
            PromptManager.ShowPrompt("GO!", 1.5f);
        }
    }
}
