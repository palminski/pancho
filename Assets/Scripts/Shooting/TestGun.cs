using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TestGun : Equipable
{
    [Tooltip("Variance Of Spawned Bullets In Degrees")][Range(0f, 360f)] public float bulletSpread;
    [Tooltip("Variance Of Spawned Bullets In Degrees While Aiming")][Range(0f, 360f)] public float bulletSpreadWhileAiming;

    public Bullet bullet;
    int ammo =100;

    int chamberIndex = 0;
    bool[] chambers = {true,true,true,true,true,true};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        string debugText = ammo + " ";
        foreach (var chamber in chambers)
        {
            if (chamber)
            {
                debugText += "[1]";
            }
            else
            {
                debugText += "[_]";
            }
        }

        debugText += " Current: " + chamberIndex + " ";
        if (chambers[chamberIndex])
        {
            debugText += "LOADED";
        }
        else
        {
            debugText += "empty";
        }
        GameController.Instance.debugText.text = debugText;
    }

    void Awake()
    {
        
    }

    public override void TriggerAction(bool isAiming = false)
    {
        Shoot(isAiming);
    }
    public override void EquippedOneAction()
    {
        LoadChamber();
    }
    public override void EquippedTwoAction()
    {
        RotateChamber();
    }

    void Shoot(bool isAiming)
    {
        Player player = GetComponentInParent<Player>();
        if (bullet != null && chambers[chamberIndex])
        {
            Bullet _bullet = Instantiate(bullet, transform.position, transform.rotation);
            float playerDIrectionAngle = Mathf.Atan2(player.directionFacing.y, player.directionFacing.x) * Mathf.Rad2Deg;
            _bullet.bulletAngle = playerDIrectionAngle;
            _bullet.bulletSpread = isAiming ? bulletSpreadWhileAiming : bulletSpread;
            chambers[chamberIndex] = false;
            RotateChamber();
        }
        else
        {
            print("empty");
        }
    }

    void RotateChamber()
    {
        chamberIndex = (chamberIndex +1) % chambers.Length;
    }

    void LoadChamber()
    {
        if (chambers[chamberIndex] == true) return;
        if (ammo < 1) return;
        chambers[chamberIndex] = true;
        ammo--;
    }
}
