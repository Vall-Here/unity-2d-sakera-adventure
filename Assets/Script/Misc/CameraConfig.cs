using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraSetup : Singleton<CameraSetup>
{
    private CinemachineVirtualCamera vcam;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        InitializeCamera();

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (GameController.Instance != null)
        {
            GameController.Instance.OnPlayerInstantiated += HandlePlayerInstantiated;
        }
    }

    private void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnPlayerInstantiated -= HandlePlayerInstantiated;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeCamera()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"CameraSetup: Scene {scene.name} loaded.");

        // Cari CinemachineVirtualCamera di scene yang baru dimuat
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam == null)
        {
            Debug.LogError("CameraSetup: CinemachineVirtualCamera tidak ditemukan di scene baru!");
            return;
        }

        Debug.Log("CameraSetup: CinemachineVirtualCamera di scene baru ditemukan.");

        // Jika player sudah diinstansiasi, set camera to follow
        if (PlayerController.Instance != null)
        {
            SetPlayerCameraFollow(PlayerController.Instance.transform);
        }
        else
        {
            Debug.LogWarning("CameraSetup: PlayerController.Instance belum ditemukan di scene baru.");
        }
    }

    /// <summary>
    /// Handler untuk event OnPlayerInstantiated
    /// </summary>
    /// <param name="player">Player yang diinstansiasi</param>
    private void HandlePlayerInstantiated(GameObject player)
    {
        if (vcam == null)
        {
            Debug.LogError("CameraSetup: CinemachineVirtualCamera masih tidak ditemukan!");
            InitializeCamera();
            if (vcam == null)
            {
                Debug.LogError("CameraSetup: CinemachineVirtualCamera masih tidak ditemukan setelah pencarian ulang!");
                return;
            }
        }

        if (player != null)
        {
            vcam.Follow = player.transform;
            Debug.Log("CameraSetup: Kamera sekarang mengikuti player.");
        }
        else
        {
            Debug.LogError("CameraSetup: Player yang diberikan adalah null!");
        }
    }

    /// <summary>
    /// Method untuk mengatur kamera mengikuti player
    /// </summary>
    public void SetPlayerCameraFollow(Transform playerTransform)
    {
        if (playerTransform != null)
        {
            if (vcam == null)
            {
                InitializeCamera();
                if (vcam == null)
                {
                    Debug.LogError("CameraSetup: CinemachineVirtualCamera tidak ditemukan!");
                    return;
                }
            }

            vcam.Follow = playerTransform;
            Debug.Log("CameraSetup: Kamera secara manual diatur untuk mengikuti player.");
        }
        else
        {
            Debug.LogError("CameraSetup: playerTransform yang diberikan adalah null!");
        }
    }
}
