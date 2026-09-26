using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public sealed class PhysicsObjectDragger2D : MonoBehaviour
{
    private const int IgnoreRaycastLayer = 2;

    [Header("Drag Settings")]
    [SerializeField, Min(0f)]
    private float springStrength = 500f;

    [SerializeField, Min(0f)]
    private float damping = 50f;

    [SerializeField, Min(0f)]
    private float maxForce = 5000f;

    [Header("Raycast Settings")]
    [SerializeField]
    private LayerMask draggableLayers;

    private Camera _camera;

    private Rigidbody2D _draggedBody;
    private Vector3 _localGrabPosition;
    private int _originalLayer;

    private bool _draggingEnabled = true;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!_draggingEnabled || Mouse.current is null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryBeginDrag();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrag();
        }
    }

    private void FixedUpdate()
    {
        if (!_draggingEnabled || _draggedBody is null)
        {
            return;
        }

        ApplyDragForce();
    }

    /// <summary>
    /// Enables or disables physics object dragging.
    /// Disabling dragging also releases the currently dragged object.
    /// </summary>
    public void SetDraggingEnabled(bool enabled)
    {
        _draggingEnabled = enabled;

        if (!enabled)
        {
            EndDrag();
        }
    }

    private void TryBeginDrag()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(mouseScreenPosition);

        bool prev = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = true;

        RaycastHit2D hit = Physics2D.GetRayIntersection(
            ray,
            Mathf.Infinity,
            draggableLayers
        );

        Physics2D.queriesHitTriggers = prev;

        if (hit.collider is null || hit.rigidbody is null)
        {
            return;
        }

        _draggedBody = hit.rigidbody;

        // Save the exact point that was clicked in local space.
        _localGrabPosition =
            _draggedBody.transform.InverseTransformPoint(hit.point);

        // Save the object's original layer and temporarily disable raycasts.
        _originalLayer = _draggedBody.gameObject.layer;
        _draggedBody.gameObject.layer = IgnoreRaycastLayer;
    }

    private void EndDrag()
    {
        if (_draggedBody is null)
        {
            return;
        }

        // Restore the object's original layer.
        _draggedBody.gameObject.layer = _originalLayer;

        _draggedBody.linearVelocity = Vector2.zero;

        _draggedBody = null;
    }

    private void ApplyDragForce()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                -_camera.transform.position.z
            )
        );

        // Convert the saved local grab point back into world space.
        Vector2 grabWorldPosition =
            _draggedBody.transform.TransformPoint(_localGrabPosition);

        Vector2 positionError =
            (Vector2)mouseWorldPosition - grabWorldPosition;

        // Accounts for linear and angular velocity at the grab point.
        Vector2 pointVelocity =
            _draggedBody.GetPointVelocity(grabWorldPosition);

        Vector2 force =
            positionError * springStrength
            - pointVelocity * damping;

        force = Vector2.ClampMagnitude(force, maxForce);

        // Applying the force at the clicked point produces torque naturally.
        _draggedBody.AddForceAtPosition(
            force,
            grabWorldPosition,
            ForceMode2D.Force
        );
    }
}