using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RaycastController))]
public class Player : MonoBehaviour
{
    private RaycastController raycastController;
    public Vector2 move;
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    
    void OnEnable()
    {
        GameController.Instance.Input.OnMoveInput += OnMoveInput;
    }

    void OnDisable()
    {
        GameController.Instance.Input.OnMoveInput -= OnMoveInput;
    }

    void Awake()
    {
        raycastController = GetComponent<RaycastController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 translateAmount = Vector2.zero;
        //Horizontal Movement
        float distanceHorizonal = Mathf.Abs(move.x) * moveSpeed;
        Vector2 directionHorizontal = new(Mathf.Sign(move.x),0);
        
        RaycastControllerResult resultHorizontal = raycastController.CastRaysHorizontal(directionHorizontal, distanceHorizonal);
        if (resultHorizontal.hit)
        {
            translateAmount.x += Mathf.Sign(move.x) * resultHorizontal.distance;
        }
        else
        {
            translateAmount.x += move.x * moveSpeed;
        }

        //Vertical Movement
        float distanceVertical = Mathf.Abs(move.y) * moveSpeed;
        Vector2 directionVertical = new(0,Mathf.Sign(move.y));
        RaycastControllerResult resultVertical = raycastController.CastRaysVertical(directionVertical, distanceVertical);
        if (resultVertical.hit)
        {
            translateAmount.y += Mathf.Sign(move.y) * resultVertical.distance;
        }
        else
        {
            translateAmount.y += move.y * moveSpeed;
        }

        rb.MovePosition(rb.position + translateAmount);

    }

    void OnMoveInput(Vector2 input)
    {
        move = input;
    }
}
