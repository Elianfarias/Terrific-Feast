using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("Signs / Mazes / Drinks")]
    [SerializeField] private List<MazeEntry> entries = new List<MazeEntry>();

    [Header("Wand Prefabs")]
    [SerializeField] private GameObject keyboardWandPrefab;
    [SerializeField] private GameObject mouseWandPrefab;
    [SerializeField] private Transform wandsParent;

    [Header("Water Prefab")]
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private Transform waterParent;

    [Header("WinZone")]
    [SerializeField] private Transform winZoneTransform;

    [Header("Glass")]
    [SerializeField] private GlassController glass;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Hint")]
    [SerializeField] private GameObject hintObject;

    [Header("Feedback")]
    [SerializeField] private CameraShake cameraShake;

    [Header("Config")]
    [SerializeField] private float waterRiseSpeed = 0.7f;
    [SerializeField] private float retryDelay = 0.6f;
    [SerializeField] private float backToSignsDelay = 1.2f;
    [SerializeField] private float signsEnableDelay = 0.4f;
    [SerializeField] private float countdownSeconds = 3f;

    public event Action<int> OnCountdownTick;
    public event Action OnCountdownFinished;
    public event Action OnMistakeFeedback;
    public event Action OnSuccessFeedback;
    private MazeEntry activeEntry;

    private GameObject keyboardWandInstance;
    private Wand keyboardWand;
    private KeyboardWandMover keyboardWandMover;

    private GameObject mouseWandInstance;
    private Wand mouseWand;
    private MouseWandMover mouseWandMover;

    private GameObject waterInstance;
    private WaterLevel water;

    private bool keyboardReachedGoal;
    private bool mouseReachedGoal;

    private Coroutine countdownRoutine;
    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        foreach (var entry in entries)
            entry.sign.OnClicked += HandleSignClicked;
    }
    private void OnDisable()
    {
        foreach (var entry in entries)
            entry.sign.OnClicked -= HandleSignClicked;

        if (countdownRoutine != null) StopCoroutine(countdownRoutine);
        CleanupWandsAndWater();
        Cursor.visible = true;
    }
    private void Start()
    {
        ShowSignSelection();
    }
    private void ShowSignSelection()
    {
        activeEntry = null;
        CleanupWandsAndWater();
        glass.Hide();
        Cursor.visible = true;

        if (hintObject != null)
            hintObject.SetActive(true);

        foreach (var entry in entries)
        {
            entry.maze.gameObject.SetActive(false);
            entry.sign.gameObject.SetActive(true);
            entry.sign.SetInteractable(false);
        }

        StartCoroutine(EnableSignsAfterDelay());
    }
    private IEnumerator EnableSignsAfterDelay()
    {
        yield return new WaitForSeconds(signsEnableDelay);
        foreach (var entry in entries)
            entry.sign.SetInteractable(true);
    }
    private void HandleSignClicked(SignButton sign)
    {
        var entry = entries.Find(e => e.sign == sign);
        if (entry == null) return;

        if (hintObject != null)
            hintObject.SetActive(false);

        foreach (var e in entries)
        {
            e.sign.SetInteractable(false);
            if (e != entry) e.sign.gameObject.SetActive(false);
        }

        StartMaze(entry);
    }
    private void StartMaze(MazeEntry entry)
    {
        activeEntry = entry;
        entry.maze.gameObject.SetActive(true);
        entry.sign.gameObject.SetActive(false);
        Cursor.visible = false;

        SpawnWandsAndWater(entry);

        // Freeze until the countdown finishes
        SetWandsControlEnabled(false);
        water.SetRising(false);

        countdownRoutine = StartCoroutine(RunCountdown());
    }
    private IEnumerator RunCountdown()
    {
        int count = Mathf.CeilToInt(countdownSeconds);
        for (int i = count; i > 0; i--)
        {
            OnCountdownTick?.Invoke(i);
            yield return new WaitForSeconds(1f);
        }

        OnCountdownFinished?.Invoke();

        SetWandsControlEnabled(true);
        if (water != null) water.SetRising(true);

        countdownRoutine = null;
    }
    private void SpawnWandsAndWater(MazeEntry entry)
    {
        CleanupWandsAndWater();

        keyboardReachedGoal = false;
        mouseReachedGoal = false;

        // --- Keyboard Wand ---
        keyboardWandInstance = Instantiate(keyboardWandPrefab, wandsParent);
        keyboardWand = keyboardWandInstance.GetComponent<Wand>();
        keyboardWandMover = keyboardWandInstance.GetComponent<KeyboardWandMover>();
        keyboardWand.Init(entry.maze.KeyboardSpawnPosition);
        keyboardWand.OnFailed += HandleFail;
        keyboardWand.OnHitWall += HandleWallHit;
        keyboardWand.OnReachedGoal += HandleKeyboardReachedGoal;

        // --- Wand Mouse ---
        mouseWandInstance = Instantiate(mouseWandPrefab, wandsParent);
        mouseWand = mouseWandInstance.GetComponent<Wand>();
        mouseWandMover = mouseWandInstance.GetComponent<MouseWandMover>();
        mouseWand.Init(entry.maze.MouseSpawnPosition);
        mouseWand.OnFailed += HandleFail;
        mouseWand.OnHitWall += HandleWallHit;
        mouseWand.OnReachedGoal += HandleMouseReachedGoal;

        StartCoroutine(WarpMouseNextFrame(entry.maze.MouseSpawnPosition));

        // --- Water ---
        waterInstance = Instantiate(waterPrefab, waterParent);
        water = waterInstance.GetComponent<WaterLevel>();
        water.Init(new Vector3(0, entry.maze.WaterBasePosition.y, 0), waterRiseSpeed);
    }
    private IEnumerator WarpMouseNextFrame(Vector3 worldPosition)
    {
        yield return null;
        WarpMouseTo(worldPosition);
    }
    private void WarpMouseTo(Vector3 worldPosition)
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null || Mouse.current == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        Mouse.current.WarpCursorPosition(new Vector2(screenPos.x, screenPos.y));
    }
    private void CleanupWandsAndWater()
    {
        if (keyboardWand != null)
        {
            keyboardWand.OnFailed -= HandleFail;
            keyboardWand.OnHitWall -= HandleWallHit;
            keyboardWand.OnReachedGoal -= HandleKeyboardReachedGoal;
        }
        if (mouseWand != null)
        {
            mouseWand.OnFailed -= HandleFail;
            mouseWand.OnHitWall -= HandleWallHit;
            mouseWand.OnReachedGoal -= HandleMouseReachedGoal;
        }

        if (keyboardWandInstance != null) Destroy(keyboardWandInstance);
        if (mouseWandInstance != null) Destroy(mouseWandInstance);
        if (waterInstance != null) Destroy(waterInstance);

        keyboardWandInstance = null;
        mouseWandInstance = null;
        waterInstance = null;
        keyboardWand = null;
        mouseWand = null;
        water = null;
        keyboardWandMover = null;
        mouseWandMover = null;
    }
    // Tocar el agua ya no reinicia el laberinto: se pierde ese trago y se
    // sigue igual al minijuego de saborizantes (con la bebida base fallida).
    private void HandleFail()
    {
        SetWandsControlEnabled(false);
        if (water != null) water.SetRising(false);

        OnMistakeFeedback?.Invoke();

        activeEntry.maze.gameObject.SetActive(false);
        CleanupWandsAndWater();
        Invoke(nameof(GoToGlyphMinigame), retryDelay);
    }

    // Tocar una pared ya no reinicia el laberinto: solo vibra la cámara.
    private void HandleWallHit()
    {
        if (cameraShake != null)
            cameraShake.Shake();
    }
    private void HandleKeyboardReachedGoal()
    {
        keyboardReachedGoal = true;
        keyboardWand.SetActiveState(false);
        keyboardWandMover.controlsEnabled = false;
        CheckWinCondition();
    }
    private void HandleMouseReachedGoal()
    {
        mouseReachedGoal = true;
        mouseWand.SetActiveState(false);
        mouseWandMover.controlsEnabled = false;
        CheckWinCondition();
    }
    private void CheckWinCondition()
    {
        if (keyboardReachedGoal && mouseReachedGoal)
            HandleWin();
    }
    private void HandleWin()
    {
        SetWandsControlEnabled(false);
        if (water != null) water.SetRising(false);

        OnSuccessFeedback?.Invoke(); // "MUY BIEN"

        glass.PlayWinSequence(activeEntry.data.fullGlassSprite, OnWinSequenceFinished);
    }
    // Se ganó el laberinto (bebida base lista): el flujo sigue en el
    // minijuego de glifos para los saborizantes, no vuelve a los carteles.
    private void OnWinSequenceFinished()
    {
        activeEntry.maze.gameObject.SetActive(false);
        CleanupWandsAndWater();
        Invoke(nameof(GoToGlyphMinigame), backToSignsDelay);
    }

    // Guarda qué vaso mostrar en el minijuego de glifos y cambia de escena.
    // La usan tanto el camino de victoria como el de fallo (agua).
    private void GoToGlyphMinigame()
    {
        if (activeEntry != null)
        {
            WandMinigameSession.SelectedGlassSprite = activeEntry.data.fullGlassSprite;
            WandMinigameSession.SelectedLiquid = activeEntry.data;
        }

        SceneManager.LoadScene("Glyph Minigame");
    }
    private void SetWandsControlEnabled(bool value)
    {
        if (keyboardWand != null) keyboardWand.SetActiveState(value);
        if (mouseWand != null) mouseWand.SetActiveState(value);
        if (keyboardWandMover != null) keyboardWandMover.controlsEnabled = value;
        if (mouseWandMover != null) mouseWandMover.controlsEnabled = value;
    }
}