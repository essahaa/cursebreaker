using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonController : MonoBehaviour
{
    public Toggle muteToggle;
    public Soundcontroller soundController;

    private void Start()
    {
        // Ensure the muteToggle and soundController references are set in the Unity Editor
        if (muteToggle == null || soundController == null)
        {
            Debug.LogError("muteToggle or soundController is not set in the Unity Editor.");
            return;
        }

        // Subscribe to the OnValueChanged event of the Toggle
        muteToggle.onValueChanged.AddListener(OnToggleValueChanged);

        Debug.Log("SettingsButtonController Start method called.");
    }

    private void OnToggleValueChanged(bool isMuted)
    {
        Debug.Log("Toggle Value Changed: " + isMuted);

        // Call the ToggleMuteSounds method in the SoundController script
        soundController.ToggleMuteSounds(isMuted);
    }
}
