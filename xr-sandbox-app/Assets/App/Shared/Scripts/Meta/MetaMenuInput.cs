using UnityEngine;
using FloatingMenuDemo; // For FloatingMenuManager

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Listens for Meta Controller input (Menu Button) to toggle the Floating Manager.
    /// Replaces the direct InputAction dependency in FloatingMenuManager for the Meta migration.
    /// </summary>
    public class MetaMenuInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FloatingMenuManager menuManager;

        [Header("Settings")]
        [Tooltip("Which button toggles the menu. Default is Start (Left Menu Button on Quest)")]
        [SerializeField] private OVRInput.Button toggleButton = OVRInput.Button.Start;
        
        [Tooltip("Which controller to listen to.")]
        [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.LTouch;

        private void Update()
        {
            if (menuManager == null) return;

            // Check for button release to toggle
            if (OVRInput.GetUp(toggleButton, controller))
            {
                menuManager.ToggleMenu();
            }
        }
    }
}
