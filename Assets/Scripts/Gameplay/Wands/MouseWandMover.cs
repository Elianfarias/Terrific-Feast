using UnityEngine;
using UnityEngine.InputSystem;
public class MouseWandMover : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float maxSpeed = 8f;
    [HideInInspector] public bool controlsEnabled = true;

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cam == null) cam = Camera.main;
    }
    private void FixedUpdate()
    {
        if (!controlsEnabled) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector3 screenPos = mouse.position.ReadValue();
        screenPos.z = -cam.transform.position.z;
        Vector2 targetPos = cam.ScreenToWorldPoint(screenPos);

        Vector2 currentPos = rb.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, maxSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
}
