using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPlayerSettings : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;

    [Header("Settings Player")]
    [SerializeField] private PlayerDataSO playerSettings;

    [Header("Buttons Setting")]
    [SerializeField] private Button btnSave;
    [SerializeField] private Button btnBack;

    [Header("Settings Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private TMP_Text txtMusicValue;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private TMP_Text txtSFXValue;
    [SerializeField] private Slider sliderUI;
    [SerializeField] private TMP_Text txtUIValue;

    private const float DB_MIN = -80f;
    private const float DB_MAX = 0f;

    private void Awake()
    {
        btnBack.onClick.AddListener(OnBackPause);
        btnSave.onClick.AddListener(OnSaveClicked);
        sliderMusic.onValueChanged.AddListener(OnValueChangeMusic);
        sliderSFX.onValueChanged.AddListener(OnValueChangeSFX);
        sliderUI.onValueChanged.AddListener(OnValueChangeUI);
    }

    private void Start()
    {
        sliderMusic.value = playerSettings.volumeMusic;
        sliderSFX.value = playerSettings.volumeSFX;
        sliderUI.value = playerSettings.volumeUI;

        OnValueChangeMusic(sliderMusic.value);
        OnValueChangeSFX(sliderSFX.value);
        OnValueChangeUI(sliderUI.value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "InGame")
            panelSettings.SetActive(false);
    }

    private void OnDestroy()
    {
        btnBack.onClick.RemoveAllListeners();
        btnSave.onClick.RemoveAllListeners();
    }

    private void OnBackPause()
    {
        panelSettings.SetActive(false);
        UIMainMenu.Instance.ToggleUIMainMenu();
    }

    private void OnSaveClicked()
    {
        playerSettings.volumeMusic = sliderMusic.value;
        playerSettings.volumeSFX = sliderSFX.value;
        playerSettings.volumeUI = sliderUI.value;

        OnBackPause();
    }

    private void OnValueChangeMusic(float v)
    {
        float vol = Mathf.InverseLerp(DB_MIN, DB_MAX, v) * 100f;

        txtMusicValue.text = vol.ToString("0");
        audioMixer.SetFloat("VolumeMusic", v);
    }

    private void OnValueChangeSFX(float v)
    {
        float vol = Mathf.InverseLerp(DB_MIN, DB_MAX, v) * 100f;

        txtSFXValue.text = vol.ToString("0");
        audioMixer.SetFloat("VolumeSFX", v);
    }

    private void OnValueChangeUI(float v)
    {
        float vol = Mathf.InverseLerp(DB_MIN, DB_MAX, v) * 100f;

        txtUIValue.text = vol.ToString("0");
        audioMixer.SetFloat("VolumeUI", v);
    }
}
