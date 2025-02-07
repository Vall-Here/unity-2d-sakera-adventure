using UnityEngine;
using TMPro; // Tambahkan ini untuk TextMeshProUGUI

public class FPSUpdater : MonoBehaviour
{
    float fps;
    float updateTimer = 0.2f;

    [SerializeField] private TextMeshProUGUI fpsText;

    private void UpdateFPSDisplay()
    {
        updateTimer -= Time.unscaledDeltaTime;
        if (updateTimer <= 0)
        {
            fps = 1.0f / Time.unscaledDeltaTime; 
            fpsText.text = "FPS: " + Mathf.Round(fps).ToString();
            updateTimer = 0.2f;
        }
    }

    void Update() 
    {
        UpdateFPSDisplay();
    }
}
