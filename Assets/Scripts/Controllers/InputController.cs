using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputController : MonoBehaviour
{
    
    
    public event Action<Vector2> OnMoveInput;
    public event Action<bool> OnAimInput;
 
    public event Action OnTriggerPressed;
    public event Action OnEquippedOnePressed;
    public event Action OnEquippedTwoPressed;
    

    public void OnMove(InputValue input)
    {
        Vector2 move = input.Get<Vector2>();
        OnMoveInput?.Invoke(move);            
    }

    public void OnAim(InputValue input)
    {
        bool isAiming = input.isPressed;
        OnAimInput?.Invoke(isAiming);
    }

    public void OnTrigger(InputValue input)
    {
        OnTriggerPressed?.Invoke();            
    }

    public void OnEquippedOne(InputValue input)
    {
        OnEquippedOnePressed?.Invoke();            
    }

    public void OnEquippedTwo(InputValue input)
    {
        OnEquippedTwoPressed?.Invoke();            
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
