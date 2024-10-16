using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class StateMachine
{
    private GameObject owner;
    private MovementState currentState;

    public StateMachine( GameObject _owner )
    {
        owner = _owner;
        SetState(new Grounded());
    }

    public void Update()
    {
        if ( currentState != null )
        {
            currentState.StateUpdate(owner);
        }
    }

    public void FixedUpdate()
    {
        if ( currentState != null )
        {
            currentState.StateFixedUpdate(owner);
        }
    }

    public void SetState( MovementState newState )
    {
        if ( currentState != null )
        {
            currentState.StateComplete(owner);
        }

        newState.StateStart(owner);

        currentState = newState;
    }

    public void APress(InputAction.CallbackContext context)
    {
        currentState.APress(context);
    }

    public void BPress(InputAction.CallbackContext context)
    {
        currentState.BPress(context);
    }

    public void XPress(InputAction.CallbackContext context)
    {
        currentState.XPress(context);
    }

    public void YPress(InputAction.CallbackContext context)
    {
        currentState.YPress(context);
    }

}

