using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    const string sceneMinigame = "Glyph Minigame";
    const string sceneVisualNovel = "VisualNovelScene";
    public static UIMainMenu Instance { get; private set; }
    public bool isPause = false;

    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelCredits;
    [SerializeField] private Image backgroundInGameImage;

    [Header("Buttons Main Menu")]
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnCredits;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnBackCredits;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        btnStart.onClick.AddListener(TogglePause);
        btnSettings.onClick.AddListener(OnSettingClicked);

        if (btnExit != null)
            btnExit.onClick.AddListener(OnExitClicked);
        if (btnCredits != null)
            btnCredits.onClick.AddListener(OnCreditClicked);
        if (btnBackCredits != null)
            btnBackCredits.onClick.AddListener(OnBackCredits);

        if (btnContinue != null)
        {
            btnContinue.onClick.AddListener(OnContinueClicked);
            btnContinue.gameObject.SetActive(File.Exists(GameStateProgress.SavePath));
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            && (SceneManager.GetActiveScene().name == sceneMinigame || SceneManager.GetActiveScene().name == sceneVisualNovel))
        {
            if (!panelMainMenu.activeSelf && isPause)
                ToggleUIMainMenu();
            else
                TogglePause();
        }
    }

    private void OnDestroy()
    {
        btnStart.onClick.RemoveAllListeners();
        btnSettings.onClick.RemoveAllListeners();

        if (btnContinue != null)
            btnContinue.onClick.RemoveAllListeners();
        if (btnCredits != null)
            btnCredits.onClick.RemoveAllListeners();
        if (btnBackCredits != null)
            btnBackCredits.onClick.RemoveAllListeners();
    }

    public void TogglePause()
    {
        if (SceneManager.GetActiveScene().name == sceneMinigame || SceneManager.GetActiveScene().name == sceneVisualNovel)
        {
            isPause = !isPause;
            backgroundInGameImage.enabled = isPause;

            if (isPause)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;

            ToggleUIMainMenu();
        }
        else
        {
            Time.timeScale = 1f;
            ResetProgress();
            SceneManager.LoadScene(sceneVisualNovel);
            ToggleUIMainMenu();
        }
    }

    // "Start" desde el Main Menu siempre arranca de cero: borra guardado.json
    // para que no quede activeChar, afectoX ni resumeNode de una partida anterior.
    private void ResetProgress()
    {
        if (File.Exists(GameStateProgress.SavePath))
            File.Delete(GameStateProgress.SavePath);
    }

    // "Continuar": va directo a la novela visual sin tocar el guardado, para
    // que YarnComands retome desde el último checkpointNode guardado.
    private void OnContinueClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneVisualNovel);
    }

    public void ToggleUIMainMenu()
    {
        if (panelCredits != null && panelCredits.activeSelf)
            panelCredits.SetActive(false);
        if (panelSettings.activeSelf)
            panelCredits.SetActive(false);

        panelMainMenu.SetActive(!panelMainMenu.activeSelf);
    }

    private void OnSettingClicked()
    {
        ToggleUIMainMenu();
        panelSettings.SetActive(true);
    }

    private void OnCreditClicked()
    {
        ToggleUIMainMenu();
        panelCredits.SetActive(true);
    }

    private void OnExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnBackCredits()
    {
        ToggleUIMainMenu();
    }
}
