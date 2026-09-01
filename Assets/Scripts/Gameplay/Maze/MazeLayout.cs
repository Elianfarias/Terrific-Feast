using UnityEngine;
public class MazeLayout : MonoBehaviour
{
    [Header("Wands spawn points")]
    [SerializeField] private Transform keyboardWandSpawn;
    [SerializeField] private Transform mouseWandSpawn;

    [Header("Water")]
    [SerializeField] private Transform waterBasePoint;

    public Vector3 KeyboardSpawnPosition =>
        keyboardWandSpawn != null ? keyboardWandSpawn.position : transform.position;
    public Vector3 MouseSpawnPosition =>
        mouseWandSpawn != null ? mouseWandSpawn.position : transform.position;
    public Vector3 WaterBasePosition =>
        waterBasePoint != null ? waterBasePoint.position : transform.position;
}