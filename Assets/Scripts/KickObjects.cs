using UnityEngine;
using UnityEngine.InputSystem;

public class KickObjects : MonoBehaviour
{
    [SerializeField] private BasicPlayer player;
    [SerializeField] private Vector2 origin;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float strength = 25f;
    [SerializeField] private LayerMask kickMask;

    public InputAction kickAction;

    private float originalOriginX;

    private void OnEnable()
    {
        kickAction.Enable();
    }

    private void OnDisable()
    {
        kickAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalOriginX = origin.x;
    }

    // Update is called once per frame
    void Update()
    {
        // kick from side facing
        origin.x = originalOriginX * player.FacingDirection;

        if (kickAction.WasPressedThisFrame())
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position + new Vector3(origin.x, origin.y), radius, Vector2.down, 0.1f, kickMask);

            foreach(RaycastHit2D hit in hits)
            {
                if(hit.rigidbody)
                    hit.rigidbody.AddForce(Vector2.up * strength,ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(origin.x, origin.y), radius);
    }
}
