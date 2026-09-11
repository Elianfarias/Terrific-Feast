using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    const string sceneMinigame = "Glyph Minigame";
    const string sceneVisualNovel = "VisualNovelScene";
    const string sceneWandMinigame = "WandMinigameScene";
    public static UIMainMenu Instance { get; private set; }
    public bool isPause = false;

    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelCredits;
    [Tooltip("Logo que solo debe verse mientras está abierto el menú principal.")]
    [SerializeField] private GameObject logo;
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

        // En el menú principal el logo está fuera del prefab de botones. Lo
        // buscamos como respaldo para que también funcione en escenas donde
        // el campo todavía no quedó asignado desde el inspector.
        if (logo == null && SceneManager.GetActiveScene().name == "MainMenu")
            logo = GameObject.Find("Logo");

        // Mantiene funcional la X aunque una variante del prefab de créditos
        // no tenga la referencia serializada en UIMainMenu.
        if (btnBackCredits == null && panelCredits != null)
            btnBackCredits = panelCredits.GetComponentInChildren<Button>(true);

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
            && (SceneManager.GetActiveScene().name == sceneMinigame 
            || SceneManager.GetActiveScene().name == sceneVisualNovel
            || SceneManager.GetActiveScene().name == sceneWandMinigame))
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
        if (SceneManager.GetActiveScene().name == sceneMinigame 
            || SceneManager.GetActiveScene().name == sceneVisualNovel
            || SceneManager.GetActiveScene().name == sceneWandMinigame)
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
        if (panelSettings != null && panelSettings.activeSelf)
            panelSettings.SetActive(false);

        panelMainMenu.SetActive(!panelMainMenu.activeSelf);
        SetLogoVisible(panelMainMenu.activeSelf);
    }

    private void OnSettingClicked()
    {
        ToggleUIMainMenu();
        panelSettings.SetActive(true);
        SetLogoVisible(false);
    }

    private void OnCreditClicked()
    {
        ToggleUIMainMenu();
        panelCredits.SetActive(true);
        SetLogoVisible(false);
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

    private void SetLogoVisible(bool visible)
    {
        if (logo != null)
            logo.SetActive(visible);
    }
}
