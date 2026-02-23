using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CastController))]
public class Player : MonoBehaviour
{
    private CastController castController;
    public Vector2 move;
    [SerializeField] private List<Equipable> equipment;
    public List<Equipable> equipmentInstances = new();

    public Vector2 directionFacing;
    public float moveSpeed = 1f;

    private int equipmentIndex = 0;
    public Equipable equipped;
    private Rigidbody2D rb;

    private bool isAiming = false;

    [SerializeField] private float diagonalHoldTime = 0.06f;
    [SerializeField] private float analogBypassMagnitude = 0.55f;

    private float diagonalHoldTimer = 0f;
    private Vector2 rawMove = Vector2.zero;
    private Vector2 lastSnapped = Vector2.zero;
    private Vector2 heldDiagonal = Vector2.zero;
    private Animator animator;

    void OnEnable()
    {
        GameController.Instance.Input.OnMoveInput += OnMoveInput;
        GameController.Instance.Input.OnAimInput += OnAimInput;
        GameController.Instance.Input.OnTriggerPressed += OnTriggerPressed;
        GameController.Instance.Input.OnRightBumperPressed += OnRightBumperPressed;
        GameController.Instance.Input.OnLeftBumperPressed += OnLeftBumperPressed;
        GameController.Instance.Input.OnEquippedOnePressed += OnEquippedOnePressed;
        GameController.Instance.Input.OnEquippedTwoPressed += OnEquippedTwoPressed;
    }

    void OnDisable()
    {
        GameController.Instance.Input.OnAimInput -= OnAimInput;
        GameController.Instance.Input.OnMoveInput -= OnMoveInput;
        GameController.Instance.Input.OnTriggerPressed -= OnTriggerPressed;
        GameController.Instance.Input.OnRightBumperPressed -= OnRightBumperPressed;
        GameController.Instance.Input.OnLeftBumperPressed -= OnLeftBumperPressed;
        GameController.Instance.Input.OnEquippedOnePressed -= OnEquippedOnePressed;
        GameController.Instance.Input.OnEquippedTwoPressed -= OnEquippedTwoPressed;
    }

    void Awake()
    {
        castController = GetComponent<CastController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        foreach (Equipable equipable in equipment)
        {
            
            if (equipable != null)
            {
                Equipable itemToInstantiate = Instantiate(equipable, transform);
                itemToInstantiate.gameObject.SetActive(false);
                equipmentInstances.Add(itemToInstantiate);
            }
            else
            {
                equipmentInstances.Add(null);
            }
        }
        Equip(equipmentInstances[equipmentIndex]);

    }

    void Update()
    {
        animator.SetBool("isMoving", move != Vector2.zero);
        if (move != Vector2.zero) animator.SetFloat("moveX", move.x);
        if (move != Vector2.zero) animator.SetFloat("moveY", move.y);
        if (diagonalHoldTimer > 0f)
        {
            diagonalHoldTimer -= Time.deltaTime;
            if (diagonalHoldTimer <= 0f)
            {
                Vector2 snapped = SnapToDirections(rawMove);
                move = snapped;
                lastSnapped = snapped;
                heldDiagonal = Vector2.zero;
            }
        }
    }


    void FixedUpdate()
    {
        if (move == Vector2.zero) return;
        directionFacing = move.normalized;

        if (isAiming) return;
        Vector2 desiredDelta = move.magnitude * moveSpeed * move.normalized;

        Vector2 resolved = ResolveWIthSliding(desiredDelta, 2);

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

            CastControllerResult hit = castController.CastBox(virtualCenter, direction, distance);

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

    Vector2 SnapToDirections(Vector2 input, int directions = 8, float deadzone = 0.15f)
    {
        float magnitude = input.magnitude;
        if (magnitude < deadzone) return Vector2.zero;

        float angle = Mathf.Atan2(input.y, input.x);
        float step = (2f * Mathf.PI) / directions;

        float snappedAngle = Mathf.Round(angle / step) * step;

        Vector2 snappedDirection = new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle));

        return snappedDirection * Mathf.Clamp01(magnitude);
    }

    bool IsDigitalLikeInput(Vector2 moveInput)
    {
        bool xInt = Mathf.Abs(moveInput.x - Mathf.Round(moveInput.x)) < 0.001f;
        bool yInt = Mathf.Abs(moveInput.y - Mathf.Round(moveInput.y)) < 0.001f;
        return xInt && yInt;
    }

    bool IsDiagonal(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;
        return Mathf.Abs(direction.x) > 0.25f && Mathf.Abs(direction.y) > 0.25f;
    }

    void Equip(Equipable itemToEquip)
    {
        GameController.Instance.debugText.text = "-";

        if (equipped != null)
        {
            equipped.gameObject.SetActive(false);
            animator.SetBool(equipped.AnimationBool, false);
            equipped = null;
        }
        if (itemToEquip != null)
        {
            itemToEquip.gameObject.SetActive(true);
            equipped = itemToEquip;
            animator.SetBool(equipped.AnimationBool, true);
        }
    }


    // =======================

    void OnMoveInput(Vector2 input)
    {

        rawMove = input;

        if (rawMove.sqrMagnitude < 0.0001f)
        {
            move = Vector2.zero;
            lastSnapped = Vector2.zero;
            heldDiagonal = Vector2.zero;
            diagonalHoldTimer = 0f;
            return;
        }

        Vector2 snapped = SnapToDirections(rawMove);

        bool wasDiagonal = IsDiagonal(lastSnapped);
        bool nowNotDiagonalButMoving = snapped != Vector2.zero && !IsDiagonal(snapped);

        bool digitalLike = IsDigitalLikeInput(rawMove);

        bool shouldHoldDiagonal = wasDiagonal && nowNotDiagonalButMoving && digitalLike;
        if (shouldHoldDiagonal)
        {
            heldDiagonal = lastSnapped;
            diagonalHoldTimer = diagonalHoldTime;
            move = heldDiagonal;
            return;
        }
        move = snapped;
        lastSnapped = snapped;
    }

    void OnAimInput(bool isPressed)
    {
        isAiming = isPressed;
    }

    void OnTriggerPressed()
    {
        if (!equipped) return;
        equipped.TriggerAction(isAiming);
    }

    void OnRightBumperPressed()
    {
        if (equipmentInstances.Count == 0) return;
        equipmentIndex = (equipmentIndex +1) % equipmentInstances.Count;
        Equip(equipmentInstances[equipmentIndex]);
    }

    void OnLeftBumperPressed()
    {
        if (equipmentInstances.Count == 0) return;
        equipmentIndex = (equipmentIndex - 1 + equipmentInstances.Count ) % equipmentInstances.Count;
        Equip(equipmentInstances[equipmentIndex]);
    }

    void OnEquippedOnePressed()
    {
        if (!equipped) return;
        equipped.EquippedOneAction();
    }

    void OnEquippedTwoPressed()
    {
        if (!equipped) return;
        equipped.EquippedTwoAction();
    }
}
