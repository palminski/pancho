using UnityEngine;

public class RaycastController : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    [SerializeField] private float skinWidth = 0.001f;
    [SerializeField] private float offsetCorrectionThreshold = 0.1f;
    public RaycastOrigins raycastOrigins;
    public float topBottomRaySpacing;
    public float sideRaySpacing;
    public LayerMask collidableLayers;
    [SerializeField] private bool shouldDrawRaysForDebug = false;
    [SerializeField] private bool shouldDrawRaysForDebug2 = false;

    [SerializeField][Min(2)] private int raysAcrossSide = 5;
    [SerializeField][Min(2)] private int raysAcrossTop = 5;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public RaycastControllerResult CastRaysHorizontal(Vector2 direction, float distance)
    {
        UpdateRaySpacing();
        UpdateRaycastOrigins();

        RaycastControllerResult result = new RaycastControllerResult();
        result.distance = float.MaxValue;
        result.hit = false;

        float raySpacing = 0;
        Vector2 rayOrigin = Vector2.zero;
        Vector2 offsetStep = Vector2.zero;


        if (direction == Vector2.right)
        {
            offsetStep = Vector2.down;
            raySpacing = sideRaySpacing;
            rayOrigin = raycastOrigins.topRight;
        }
        else if (direction == Vector2.left)
        {
            offsetStep = Vector2.up;
            raySpacing = sideRaySpacing;
            rayOrigin = raycastOrigins.bottomLeft;
        }

        for (int i = 0; i < raysAcrossSide; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin + (offsetStep * raySpacing * i),
                direction,
                distance,
                collidableLayers
            );

            if (shouldDrawRaysForDebug) Debug.DrawRay(rayOrigin + (offsetStep * raySpacing * i), direction, Color.rebeccaPurple, distance);

            if (hit.collider)
            {
                result.hitNumber += 1;
                result.hit = true;
                if (hit.distance < result.distance)
                {
                    result.normal = hit.normal;
                    result.distance = hit.distance - skinWidth;
                }
            }
        }
        return result;
    }

    public RaycastControllerResult CastRaysVertical(Vector2 direction, float distance)
    {
        UpdateRaySpacing();
        UpdateRaycastOrigins();

        RaycastControllerResult result = new RaycastControllerResult();
        result.distance = float.MaxValue;
        result.hit = false;

        float raySpacing = 0;
        Vector2 rayOrigin = Vector2.zero;
        Vector2 offsetStep = Vector2.zero;

        if (direction == Vector2.up)
        {
            offsetStep = Vector2.right;
            raySpacing = topBottomRaySpacing;
            rayOrigin = raycastOrigins.topLeft;
        }
        else if (direction == Vector2.down)
        {
            offsetStep = Vector2.left;
            raySpacing = topBottomRaySpacing;
            rayOrigin = raycastOrigins.bottomRight;
        }

        for (int i = 0; i < raysAcrossTop; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin + (offsetStep * raySpacing * i),
                direction,
                distance,
                collidableLayers
            );

            if (shouldDrawRaysForDebug) Debug.DrawRay(rayOrigin + (offsetStep * raySpacing * i), direction, Color.rebeccaPurple, distance);

            if (hit.collider)
            {
                result.hitNumber += 1;
                result.hit = true;
                if (hit.distance < result.distance)
                {
                    result.normal = hit.normal;
                    result.distance = hit.distance - skinWidth;
                }
            }
        }
        return result;
    }



    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    public void TranslateRaycastOrigins(Vector2 translateAmount)
    {
        raycastOrigins.topRight += translateAmount;
        raycastOrigins.topLeft += translateAmount;
        raycastOrigins.bottomLeft += translateAmount;
        raycastOrigins.bottomRight += translateAmount;
    }

    public void UpdateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        sideRaySpacing = bounds.size.x / (raysAcrossSide - 1);
        topBottomRaySpacing = bounds.size.y / (raysAcrossTop - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
}

public struct RaycastControllerResult
{
    public bool hit;
    public float distance;
    public int hitNumber;
    public Vector2 normal;
}
