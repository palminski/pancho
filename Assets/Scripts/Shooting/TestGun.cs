using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestGun : Equipable
{
    public Bullet bullet;
    int ammo =100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Awake()
    {
        
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Player player = GetComponentInParent<Player>();
        if (bullet != null && ammo > 0)
        {
            Bullet _bullet = Instantiate(bullet, transform.position, transform.rotation);
            float playerDIrectionAngle = Mathf.Atan2(player.directionFacing.y, player.directionFacing.x) * Mathf.Rad2Deg;
            _bullet.bulletAngle = playerDIrectionAngle;
            ammo--;
            print(ammo);
        }
        else
        {
            print("empty");
        }
    }
}
