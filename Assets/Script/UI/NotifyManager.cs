using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField]
    private GameObject notificationPanel;
    [SerializeField]
    private TextMeshProUGUI notificationText;

    [SerializeField]
    private float displayDuration = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        StartCoroutine(ShowNotificationCoroutine(message));
    }

    private IEnumerator ShowNotificationCoroutine(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);

        // Tunggu selama durasi yang ditentukan
        yield return new WaitForSeconds(displayDuration);

        notificationPanel.SetActive(false);
    }
}
