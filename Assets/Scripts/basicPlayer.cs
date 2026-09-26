using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BasicPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float airSpeedMult = 0.8f;
    [SerializeField] private float jumpForce = 25;

    [Header("Leg Config")]

    [SerializeField] private Transform hip;
    [SerializeField] private float footSize = 0.1f;
    [SerializeField] private float legHeight = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float restRatio = 0.7f;
    [SerializeField] float kp = 100f;
    [SerializeField] float ki = 10f;
    [SerializeField] float kd = 20f;
    [SerializeField] private LayerMask walkableMask;
    public InputAction moveAction;
    public InputAction jumpAction;
    private Rigidbody2D rb;
    private float legI;
    private float legError;
    public Vector2 FootPos { get; private set; }
    public float FacingDirection { get; private set; }
    private bool isGrounded;

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
        FacingDirection = 1;
    }

    private void Update()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = 0;
        float moveMult = 1;

        isGrounded = IsGrounded(out dist);

        if (isGrounded)
        {

            moveMult *= airSpeedMult;

            // apply hover force
            hoverPIDF(dist);
        }
        else
        {
            dist = legHeight;
        }

        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        rb.AddForce(Vector2.right * moveDirection * moveSpeed * moveMult);

        FootPos = hip.position - new Vector3(0, dist, 0);

        if (moveDirection.x != 0)
        {
            FacingDirection = moveDirection.x / Mathf.Abs(moveDirection.x);
        }

        //if (moveDirection.x != 0)
        //{
        //    FacingDirection = rb.linearVelocity.x / Mathf.Abs(rb.linearVelocity.x);
        //}
    }

    void hoverPIDF(float dist)
    {
        float targetHeight = legHeight * restRatio;
        float error = targetHeight - dist;

        legI += error * Time.fixedDeltaTime;

        float derivative = (error - legError) / Time.fixedDeltaTime;

        float force = rb.mass * Physics2D.gravity.magnitude
            + kp * error
            + ki * legI
            - kd * rb.linearVelocity.y;

        legError = error;

        force = Mathf.Max(force, 0f);

        rb.AddForce(Vector2.up * force);
    }

    bool IsGrounded(out float dist)
    {
        RaycastHit2D hit = Physics2D.CircleCast(hip.position, footSize, Vector2.down, legHeight, walkableMask);

        dist = hit.distance;
        return hit;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hip.position, footSize);
        Gizmos.DrawSphere(hip.position + Vector3.down * legHeight, footSize);
        Gizmos.DrawLine(hip.position + Vector3.right * footSize, hip.position + Vector3.down * legHeight + Vector3.right * footSize);
        Gizmos.DrawLine(hip.position + Vector3.left * footSize, hip.position + Vector3.down * legHeight + Vector3.left * footSize);
        float dist = 0;
        IsGrounded(out dist);
        Gizmos.color = Color.rebeccaPurple;
        Gizmos.DrawLine(hip.position + Vector3.down * dist + Vector3.right * footSize, hip.position + Vector3.down * dist + Vector3.left * footSize);
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawLine(hip.position + Vector3.down * legHeight * restRatio + Vector3.right * footSize, hip.position + Vector3.down * legHeight * restRatio + Vector3.left * footSize);
    }
}
