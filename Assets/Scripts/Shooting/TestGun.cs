using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestGun : Equipable
{
    DebugScript ds;

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
        print("Shoot!");
        ds= GetComponent<DebugScript>();
        if(ds == null) return;
        ds.Debug();
    }
}
