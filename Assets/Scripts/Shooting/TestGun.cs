using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestGun : Equipable
{
    DebugScript ds;

    int ammo =1000;

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
        ammo--;
        print("Shoot!");
        print(ammo);
    }
}
