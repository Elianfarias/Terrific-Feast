using UnityEngine;
using UnityEngine.InputSystem;
public class KeyboardWandMover : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [HideInInspector] public bool controlsEnabled = true;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled) return;

        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.wKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed) input.y -= 1;
        if (kb.aKey.isPressed) input.x -= 1;
        if (kb.dKey.isPressed) input.x += 1;

        rb.MovePosition(rb.position + input.normalized * speed * Time.fixedDeltaTime);
    }
}