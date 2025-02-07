using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string areaTransitionName;

    private void Start()
    {
        if (areaTransitionName == SceneManagement.Instance.AreaTransitionName)
        {
            PlayerController.Instance.transform.position = this.transform.position;

            if (PlayerController.Instance != null)
            {
                CameraSetup.Instance.SetPlayerCameraFollow(PlayerController.Instance.transform);
            }
            UIFade.Instance.FadeFromBlack();

            if (DayNightCycle.Instance != null)
            {
                DayNightCycle.Instance.ResetReferences();
                Debug.Log("AreaEntrance: DayNightCycle references telah direset.");
            }
            else
            {
                Debug.LogError("AreaEntrance: DayNightCycle.Instance tidak ditemukan!");
            }
        }
    }
}
