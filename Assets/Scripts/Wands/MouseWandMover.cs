using UnityEngine;
using UnityEngine.InputSystem;
public class MouseWandMover : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector3 screenPos = mouse.position.ReadValue();
        screenPos.z = -cam.transform.position.z;
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        rb.MovePosition(worldPos);
    }
}