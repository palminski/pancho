using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputController : MonoBehaviour
{
    
    public Vector2 Move {get; private set;}
    public event Action<Vector2> OnMoveInput;
    public event Action OnUseEquippedItemInput;
    

    public void OnMove(InputValue input)
    {
        Vector2 move = input.Get<Vector2>();
        
        OnMoveInput?.Invoke(move);            
    }

    public void OnUseEquippedItem(InputValue input)
    {
        print("buttonPressed");
        OnUseEquippedItemInput?.Invoke();            
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
