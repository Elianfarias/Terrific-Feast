using System.Collections;
using TMPro;
using UnityEngine;
public class MazeFeedbackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TMP_Text countdownText;

    [Header("Result Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private string mistakeMessage = "UY...";
    [SerializeField] private string successMessage = "MUY BIEN!";
    [SerializeField] private float feedbackDuration = 1.2f;

    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (countdownPanel != null) countdownPanel.SetActive(false);
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
    }
    private void OnEnable()
    {
        if (gameManager == null) return;

        gameManager.OnCountdownTick += ShowCountdownNumber;
        gameManager.OnCountdownFinished += HideCountdown;
        gameManager.OnMistakeFeedback += ShowMistake;
        gameManager.OnSuccessFeedback += ShowSuccess;
    }
    private void OnDisable()
    {
        if (gameManager == null) return;

        gameManager.OnCountdownTick -= ShowCountdownNumber;
        gameManager.OnCountdownFinished -= HideCountdown;
        gameManager.OnMistakeFeedback -= ShowMistake;
        gameManager.OnSuccessFeedback -= ShowSuccess;
    }
    private void ShowCountdownNumber(int number)
    {
        if (countdownPanel != null) countdownPanel.SetActive(true);
        if (countdownText != null) countdownText.text = number.ToString();
    }
    private void HideCountdown()
    {
        if (countdownPanel != null) countdownPanel.SetActive(false);
    }
    private void ShowMistake() => ShowFeedback(mistakeMessage);
    private void ShowSuccess() => ShowFeedback(successMessage);
    private void ShowFeedback(string message)
    {
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(FeedbackRoutine(message));
    }
    private IEnumerator FeedbackRoutine(string message)
    {
        if (feedbackText != null) feedbackText.text = message;
        if (feedbackPanel != null) feedbackPanel.SetActive(true);

        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        feedbackRoutine = null;
    }
}