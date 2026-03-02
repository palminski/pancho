using UnityEngine;

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
}

public class Enemy : HitReceivable
{
    private EnemyState enemyState = EnemyState.Walk;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }

    void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Walk:
                HandleWalk();
                break;
        }
    }

    public void DamageEnemy()
    {
        Destroy(gameObject);
    }

    public override void OnHit(HitInfo hit)
    {
        DamageEnemy();
    }

    public void HandleIdle()
    {

    }
    public void HandleAttack()
    {

    }
    public virtual void HandleWalk()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.KillPlayer();
        }
    }
}
