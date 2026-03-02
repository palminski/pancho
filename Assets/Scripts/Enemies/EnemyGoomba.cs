using UnityEngine;

public class EnemyGoomba : Enemy
{
    public float moveSpeed = 1f;
    public Vector2 startMove;
    private Vector2 move;
    private CastController castController;
    private Rigidbody2D rb;

    public override void HandleWalk()
    {
        if (move == Vector2.zero) return;

        Vector2 desiredDelta = move.magnitude * moveSpeed * move.normalized;
        Vector2 direction = desiredDelta.normalized;
        float distance = desiredDelta.magnitude;

        Vector2 virtualCenter = castController.boxCollider.bounds.center;
        CastControllerResult hit = castController.CastBox(virtualCenter, direction, distance);

        if (hit.hit)
        {
            Vector2 moveToHit = direction * hit.distance;
            desiredDelta = moveToHit;
            move *= -1;
        }

        rb.MovePosition(rb.position + desiredDelta);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        castController = GetComponent<CastController>();
        move = startMove;
        rb = GetComponent<Rigidbody2D>();
    }


}
