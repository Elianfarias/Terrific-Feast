using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Config")]
    [SerializeField] private float waterRiseSpeed = 0.1f;
    [SerializeField] private float retryDelay = 0.6f;
    [SerializeField] private float backToSignsDelay = 1.2f;

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

        CleanupWandsAndWater();
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

        foreach (var entry in entries)
        {
            entry.maze.gameObject.SetActive(false);
            entry.sign.gameObject.SetActive(true);
            entry.sign.SetInteractable(true);
        }
    }
    private void HandleSignClicked(SignButton sign)
    {
        var entry = entries.Find(e => e.sign == sign);
        if (entry == null) return;

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

        winZoneTransform.position = entry.maze.WinZonePosition;

        SpawnWandsAndWater(entry);

        glass.ShowEmptyGlass();
        SetWandsControlEnabled(true);
        water.SetRising(true);
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
        keyboardWand.OnReachedGoal += HandleKeyboardReachedGoal;

        // --- Wand Mouse ---
        mouseWandInstance = Instantiate(mouseWandPrefab, wandsParent);
        mouseWand = mouseWandInstance.GetComponent<Wand>();
        mouseWandMover = mouseWandInstance.GetComponent<MouseWandMover>();
        mouseWand.Init(entry.maze.MouseSpawnPosition);
        mouseWand.OnFailed += HandleFail;
        mouseWand.OnReachedGoal += HandleMouseReachedGoal;

        StartCoroutine(WarpMouseNextFrame(entry.maze.MouseSpawnPosition));

        // --- Water ---
        waterInstance = Instantiate(waterPrefab, waterParent);
        water = waterInstance.GetComponent<WaterLevel>();
        water.Init(entry.maze.WaterBasePosition, waterRiseSpeed);
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
            keyboardWand.OnReachedGoal -= HandleKeyboardReachedGoal;
        }
        if (mouseWand != null)
        {
            mouseWand.OnFailed -= HandleFail;
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
    private void HandleFail()
    {
        SetWandsControlEnabled(false);
        if (water != null) water.SetRising(false);
        Invoke(nameof(RetryActiveMaze), retryDelay);
    }
    private void RetryActiveMaze()
    {
        if (activeEntry == null) return;
        StartMaze(activeEntry);
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

        glass.PlayWinSequence(activeEntry.data.fullGlassSprite, OnWinSequenceFinished);
    }
    private void OnWinSequenceFinished()
    {
        activeEntry.maze.gameObject.SetActive(false);
        CleanupWandsAndWater();
        Invoke(nameof(ShowSignSelection), backToSignsDelay);
    }
    private void SetWandsControlEnabled(bool value)
    {
        if (keyboardWand != null) keyboardWand.SetActiveState(value);
        if (mouseWand != null) mouseWand.SetActiveState(value);
        if (keyboardWandMover != null) keyboardWandMover.controlsEnabled = value;
        if (mouseWandMover != null) mouseWandMover.controlsEnabled = value;
    }
}