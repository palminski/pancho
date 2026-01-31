using UnityEngine;

public class TestTarget : HitReceivable
{

    private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnHit(HitInfo hit)
    {
        sr.color = Random.ColorHSV(
            0f,1f,
            0.6f, 1f,
            0.6f, 1f
        );
    }
}
