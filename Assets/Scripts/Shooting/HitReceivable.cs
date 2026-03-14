using UnityEngine;

public interface IHitReceivable
{
    void OnHit(HitInfo hit);
}

public struct HitInfo
{
    public float damage;
}

public class HitReceivable : MonoBehaviour, IHitReceivable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnHit(HitInfo hit)
    {
    }
}
