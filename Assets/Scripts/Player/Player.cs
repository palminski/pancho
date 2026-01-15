using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CastController))]
public class Player : MonoBehaviour
{
    private CastController castController;
    public Vector2 move;
    public float moveSpeed = 1f;

    public Equipable equipped;
    private Rigidbody2D rb;
    
    void OnEnable()
    {
        GameController.Instance.Input.OnMoveInput += OnMoveInput;
        GameController.Instance.Input.OnUseEquippedItemInput += OnUseEquippedItemInput;
    }

    void OnDisable()
    {
        GameController.Instance.Input.OnMoveInput -= OnMoveInput;
        GameController.Instance.Input.OnUseEquippedItemInput -= OnUseEquippedItemInput;
    }

    void Awake()
    {
        castController = GetComponent<CastController>();
        rb = GetComponent<Rigidbody2D>();
        equipped = Instantiate(equipped, transform);
    }


    void FixedUpdate()
    {
        if(move == Vector2.zero)return;
        
        Vector2 desiredDelta = move.magnitude * moveSpeed * move.normalized;

        Vector2 resolved = ResolveWIthSliding(desiredDelta,2);
        
        rb.MovePosition(rb.position + resolved);
    }

    Vector2 ResolveWIthSliding(Vector2 delta, int maxIterations)
    {
        Vector2 remaining = delta;
        Vector2 totalMoved = Vector2.zero;
        Vector2 virtualCenter = castController.boxCollider.bounds.center;
        for (int i = 0; i < maxIterations; i++)
        {
            if (remaining.sqrMagnitude < 1e-10f) break;

            Vector2 direction = remaining.normalized;
            float distance = remaining.magnitude;

            CastControllerResult hit = castController.CastBox(virtualCenter,direction, distance);

            if (!hit.hit)
            {
                totalMoved += remaining;
                break;
            }
            Vector2 moveToHit = direction * hit.distance;
            totalMoved += moveToHit;
            virtualCenter += moveToHit;

            Vector2 leftover = remaining - moveToHit;

            remaining = leftover - hit.normal * Vector2.Dot(leftover, hit.normal);

            if (remaining.sqrMagnitude < 1e-8f) break;

        }

        return totalMoved;
    }
    
    Vector2 SnapToDirections(Vector2 input, int directions = 16, float deadzone = 0.15f)
    {
        float magnitude = input.magnitude;
        if (magnitude < deadzone) return Vector2.zero;

        float angle = Mathf.Atan2(input.y, input.x);
        float step = (2f * Mathf.PI) /directions;

        float snappedAngle = Mathf.Round(angle/step) * step;

        Vector2 snappedDirection = new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle));

        return snappedDirection * Mathf.Clamp01(magnitude);
    }

    void OnMoveInput(Vector2 input)
    {
        move = SnapToDirections(input);
    }

    void OnUseEquippedItemInput()
    {
        if(!equipped) return;
        equipped.Use();
    }
}
