using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float speed;
    [Tooltip("Direction Of Bullet In Degrees")][Range(0f, 360f)] public float bulletAngle;
    [Tooltip("Variance Of Bullet In Degrees")][Range(0f, 360f)] public float bulletSpread;
    public LayerMask collidableLayers;
    [HideInInspector] public Vector2 direction;
    private Rigidbody2D rb;
    private List<IHitReceivable> hitObjects = new();
    private Collider2D boxCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Obtain Direction From Bullet Angle
        float finalDirectionDegrees = bulletAngle + Random.Range(-bulletSpread, bulletSpread);
        float directionRadian = finalDirectionDegrees * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(directionRadian), Mathf.Sin(directionRadian));

        //Points Bullet In That Direction
        rb.SetRotation(finalDirectionDegrees);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 desiredDelta = direction.normalized * speed;

        RaycastHit2D hit = Physics2D.BoxCast(
          boxCollider.bounds.center,
          boxCollider.bounds.size,
            rb.rotation,
            direction.normalized,
            speed,
            collidableLayers
        );

        float distanceForEnemyCheck = speed;
        if (hit)
        {
            distanceForEnemyCheck = hit.distance;
        }

        //Check Enemies
        RaycastHit2D[] hitEntities = Physics2D.BoxCastAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            rb.rotation,
            direction.normalized,
            hit.distance,
            Physics2D.AllLayers
        );
        
        foreach (RaycastHit2D hitEntity in hitEntities)
        {
            if (hitEntity.collider.TryGetComponent<IHitReceivable>(out var reciever))
            {
                if (hitObjects.Contains(reciever)) continue;
                hitObjects.Add(reciever);
                HitInfo hitInfo = new HitInfo();
                reciever.OnHit(hitInfo);
            }
        }


        if (hit)
        {

            Destroy(gameObject);
        }
        else
        {
            rb.MovePosition(rb.position + desiredDelta);
        }


    }


}
