using UnityEngine;
using UnityEngine.UI;

public interface IEquipable
{
    string AnimationBool {get;}
    void TriggerAction(bool isAiming);
    void EquippedOneAction();
    void EquippedTwoAction();
}

public class Equipable : MonoBehaviour, IEquipable
{
    [SerializeField] protected string animationBool = "none";
    public virtual string AnimationBool => animationBool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TriggerAction(bool isAiming)
    {
        print("Trigger Action");
    }

    public virtual void EquippedOneAction()
    {
        print("Button Action One");
    }

    public virtual void EquippedTwoAction()
    {
        print("Button Action Two");
    }
}
