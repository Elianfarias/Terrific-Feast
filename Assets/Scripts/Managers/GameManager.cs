using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Wand keyboardWand;
    [SerializeField] private Wand mouseWand;
    private void Awake()
    {
        if (keyboardWand == null || mouseWand == null) ;
    }
    private void OnEnable()
    {
        keyboardWand.OnFailed += HandleFail;
        mouseWand.OnFailed += HandleFail;
    }
    private void OnDisable()
    {
        keyboardWand.OnFailed -= HandleFail;
        mouseWand.OnFailed -= HandleFail;
    }
    private void HandleFail()
    {
        Debug.Log("You lost!");
    }
}