using UnityEngine;
using UnityEngine.U2D;

public class SoftSprite : MonoBehaviour
{
    private const float splineOffset = 0.5f;

    [SerializeField]
    private SpriteShapeController spriteShape;

    [SerializeField] private Transform[] points;

    private void Awake()
    {
        UpdateVerts();
    }

    private void Update()
    {
        UpdateVerts();
    }

    private void UpdateVerts()
    {
        for (int i = 0; i < points.Length; i++)
        {
            {
                Transform t = points[points.Length - i - 1];

                Vector2 vertex = t.localPosition;
                Vector2 towardCenter = (Vector2.zero - vertex).normalized;
                float colliderRadius = t.gameObject.GetComponent<CircleCollider2D>().radius;

                try
                {
                    spriteShape.spline.SetPosition(i, vertex - towardCenter * colliderRadius);
                }
                catch
                {
                    spriteShape.spline.SetPosition(i, vertex - towardCenter * (colliderRadius + splineOffset));
                }

                Vector2 rt = spriteShape.spline.GetRightTangent(i);

                Vector2 newRt = Vector2.Perpendicular(towardCenter) * rt.magnitude;
                Vector2 newLt = Vector2.zero - newRt;

                spriteShape.spline.SetRightTangent(i, newRt);
                spriteShape.spline.SetLeftTangent(i, newLt);
            }
        }
    }
}