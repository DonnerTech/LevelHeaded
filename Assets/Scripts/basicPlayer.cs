using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class basicPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float jumpForce = 25;

    public InputAction moveAction;
    public InputAction jumpAction;

    private Rigidbody2D rb;

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();

        Debug.Log(moveDirection);

        rb.AddForce(Vector2.right * moveDirection * moveSpeed);

        // Read instantaneous button triggers
    }
}
