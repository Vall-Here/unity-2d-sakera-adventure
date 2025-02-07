using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float cycleDuration = 12000f; 
    private float timeElapsed = 450f; 
    private bool isDay = true;

    [Header("References")]
    public Camera mainCamera;
    public Color dayColor = Color.white; 
    public Color nightColor = Color.blue; 
    public GameObject globalLight; 
    public GameObject pointLightParent;
    public TMP_Text timeUIText; 

    [Header("Transition Settings")]
    public float transitionDuration = 5f; 
    private Color targetColor;
    private bool isTransitioningColor = false;
    private float transitionTime = 0f;

    // Singleton Instance
    public static DayNightCycle Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "HomeScreen")
        {
            return;
        }

        // Inisialisasi state awal jika diperlukan
        InitializeState();

        InitializeReferences();
        ApplyDayNightState();
        UpdateTimeUI();
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "HomeScreen") // Pastikan nama scene sesuai
        {
            return;
        }

        timeElapsed += Time.deltaTime;

        float inGameHour = (timeElapsed / cycleDuration) * 24f;

        if (timeElapsed >= cycleDuration)
        {
            timeElapsed = 0f;
            inGameHour = 0f;
        }

        bool currentIsDay = inGameHour >= 6f && inGameHour < 20f;

        // Tambahkan debug log untuk memantau nilai waktu
        // Debug.Log($"In-Game Hour: {inGameHour}, isDay: {currentIsDay}");

        if (currentIsDay != isDay)
        {
            isDay = currentIsDay;
            ToggleDayNight();
        }

        // Handle transisi warna global light
        if (isTransitioningColor)
        {
            transitionTime += Time.deltaTime;
            float t = transitionTime / transitionDuration;
            t = Mathf.Clamp01(t);

            if (globalLight != null)
            {
                Light2D globalLightComponent = globalLight.GetComponent<Light2D>();
                if (globalLightComponent != null)
                {
                    globalLightComponent.color = Color.Lerp(globalLightComponent.color, targetColor, t);
                }
            }

            if (transitionTime >= transitionDuration)
            {
                isTransitioningColor = false;
            }
        }

        // Update UI waktu
        UpdateTimeUI();
    }

    /// <summary>
    /// Inisialisasi state awal cycle jika diperlukan.
    /// </summary>
    void InitializeState()
    {
        // Anda dapat mengatur state awal di sini jika diperlukan
        // Contoh:
        // timeElapsed = 450f;
        // isDay = true;
    }

    /// <summary>
    /// Menginisialisasi referensi GameObject berdasarkan tag.
    /// </summary>
    void InitializeReferences()
    {
        // Cari Global Light
        if (globalLight == null)
        {
            GameObject[] globalLights = GameObject.FindGameObjectsWithTag("GlobalLight");
            if (globalLights.Length > 0)
            {
                globalLight = globalLights[0];
                // Debug.Log("DayNightCycle: GlobalLight ditemukan dan diassign.");
            }
            else
            {
                // Debug.LogWarning("DayNightCycle: Tidak ditemukan GlobalLight dengan tag 'GlobalLight'.");
            }
        }

        // Cari Parent Point Light
        if (pointLightParent == null)
        {
            GameObject[] pointLights = GameObject.FindGameObjectsWithTag("PointLight");
            if (pointLights.Length > 0)
            {
                pointLightParent = pointLights[0];
                // Debug.Log("DayNightCycle: PointLightParent ditemukan dan diassign.");
            }
            else
            {
                // Debug.LogWarning("DayNightCycle: Tidak ditemukan PointLightParent dengan tag 'PointLight'.");
            }
        }

        // Cari Time UI Text
        if (timeUIText == null)
        {
            GameObject timeUITextObject = GameObject.FindWithTag("ClockTag");
            if (timeUITextObject != null)
            {
                timeUIText = timeUITextObject.GetComponent<TMP_Text>();
                if (timeUIText != null)
                {
                    Debug.Log("DayNightCycle: TimeUIText ditemukan dan diassign.");
                }
                else
                {
                    // Debug.LogWarning("DayNightCycle: Komponen TMP_Text tidak ditemukan pada objek dengan tag 'ClockTag'.");
                }
            }
            else
            {
                // Debug.LogWarning("DayNightCycle: Tidak ditemukan objek dengan tag 'ClockTag'.");
            }
        }

        // Pastikan mainCamera terassign
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Debug.Log("DayNightCycle: mainCamera diassign ke Camera.main.");
            }
            else
            {
                // Debug.LogWarning("DayNightCycle: Tidak ditemukan Camera utama.");
            }
        }
    }

    /// <summary>
    /// Mengaktifkan atau menonaktifkan PointLightParent berdasarkan status siang/malam.
    /// </summary>
    void ToggleDayNight()
    {
        if (isDay)
        {
            targetColor = dayColor;
            SetPointLightsActive(false);
        }
        else
        {
            targetColor = nightColor;
            SetPointLightsActive(true);
        }

        isTransitioningColor = true;
        transitionTime = 0f;

        // Debug.Log($"DayNightCycle: Status hari sekarang: {(isDay ? "Siang" : "Malam")}.");
    }

    /// <summary>
    /// Menerapkan status siang/malam saat inisialisasi atau reset.
    /// </summary>
    void ApplyDayNightState()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "HomeScreen")
        {
            // Debug.Log("DayNightCycle: Scene 'HomeScreen'. Tidak menerapkan state siang/malam.");
            return;
        }

        if (isDay)
        {
            targetColor = dayColor;
            SetPointLightsActive(false);
        }
        else
        {
            targetColor = nightColor;
            SetPointLightsActive(true);
        }

        if (globalLight != null)
        {
            Light2D globalLightComponent = globalLight.GetComponent<Light2D>();
            if (globalLightComponent != null)
            {
                globalLightComponent.color = targetColor;
                // Debug.Log("DayNightCycle: GlobalLight color diupdate sesuai state.");
            }
            else
            {
                // Debug.LogWarning("DayNightCycle: Komponen Light2D tidak ditemukan pada GlobalLight.");
            }
        }
        else
        {
            // Debug.LogWarning("DayNightCycle: globalLight masih null saat ApplyDayNightState dipanggil.");
        }
    }

    /// <summary>
    /// Mengaktifkan atau menonaktifkan parent Point Light.
    /// </summary>
    /// <param name="isActive">Status aktif atau tidaknya PointLightParent.</param>
    void SetPointLightsActive(bool isActive)
    {
        if (pointLightParent != null)
        {
            pointLightParent.SetActive(isActive);
            // Debug.Log($"DayNightCycle: PointLightParent diaktifkan: {isActive}");
        }
        else
        {
            // Debug.LogWarning("DayNightCycle: pointLightParent belum diassign.");
        }
    }

    /// <summary>
    /// Memperbarui UI waktu di layar.
    /// </summary>
    void UpdateTimeUI()
    {
        if (timeUIText == null) return;

        float inGameHour = (timeElapsed / cycleDuration) * 24f;
        if (inGameHour >= 24f)
            inGameHour -= 24f;

        int hour = Mathf.FloorToInt(inGameHour);
        int minute = Mathf.FloorToInt((inGameHour - hour) * 60f);

        string amPm = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;

        string timeString = string.Format("{0:00}:{1:00} {2}", displayHour, minute, amPm);
        timeUIText.text = timeString;
    }

    /// <summary>
    /// Menangani event ketika scene baru dimuat.
    /// </summary>
    /// <param name="scene">Scene yang dimuat.</param>
    /// <param name="mode">Mode load scene.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log($"DayNightCycle: Scene '{scene.name}' dimuat.");

        if (scene.name == "HomeScreen")
        {
            return;
        }

        ResetReferences(); 
    }

    /// <summary>
    /// Mereset referensi saat scene baru dimuat.
    /// </summary>
    public void ResetReferences()
    {
        // Debug.Log("DayNightCycle: ResetReferences dipanggil.");
        InitializeReferences();
        ApplyDayNightState();
        UpdateTimeUI();
    }

    /// <summary>
    /// Memuat data waktu yang disimpan.
    /// </summary>
    /// <param name="savedIsDay">Status apakah saat itu siang.</param>
    /// <param name="savedTimeElapsed">Waktu yang telah berlalu dalam siklus.</param>
    /// <param name="savedCycleDuration">Durasi siklus.</param>
    public void LoadTime(bool savedIsDay, float savedTimeElapsed, float savedCycleDuration)
    {
        isDay = savedIsDay;
        timeElapsed = savedTimeElapsed;
        cycleDuration = savedCycleDuration;
        ApplyDayNightState();
    }

    /// <summary>
    /// Mendapatkan data waktu saat ini.
    /// </summary>
    /// <returns>Tuple yang berisi status siang, waktu yang telah berlalu, dan durasi siklus.</returns>
    public (bool, float, float) GetTimeData()
    {
        return (isDay, timeElapsed, cycleDuration);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
