using UnityEngine;

    public enum Turno{
    manana,
    tarde,
    noche
    }
public class GameState : MonoBehaviour
{
    public Turno turno;
    public static GameState Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
}
}