
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpHold;
    public UnityEvent<int> moveCheck;
    public UnityEvent attack;
    public UnityEvent test;

    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("JumpHold was started");
        else if (context.performed)
        {
            Debug.Log("JumpHold was performed");
            Debug.Log(context.duration);
            jumpHold.Invoke();
        }
        else if (context.canceled)
            Debug.Log("JumpHold was cancelled");
    }

    // called twice, when pressed and unpressed
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Jump was started");
        else if (context.performed)
        {
            jump.Invoke();
            Debug.Log("Jump was performed");
        }
        else if (context.canceled)
            Debug.Log("Jump was cancelled");

    }

    // For automatic movement when the button is pressed in a single direction in the 1d axis
    // public void OnMoveAction(InputAction.CallbackContext context)
    // {
    //     // Debug.Log("OnMoveAction callback invoked");
    //     if (context.started || context.performed)
    //     {
    //         Debug.Log("move started or performed");
    //         int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
    //         moveCheck.Invoke(faceRight);
    //     }
    // }

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("OnMoveAction callback invoked");
        if (context.started)
        {
            Debug.Log("move started");
            int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            moveCheck.Invoke(faceRight);
        }
        if (context.canceled)
        {
            Debug.Log("move stopped");
            moveCheck.Invoke(0);
        }

    }


    public void OnAttackAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attack.Invoke();
            Debug.Log("Attack was performed");
        }
    }
}
