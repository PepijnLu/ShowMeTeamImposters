using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class StateMachine
{
    private GameObject owner;
    public MovementState currentMovementState;
    public AttackState currentAttackState;

    public StateMachine( GameObject _owner )
    {
        owner = _owner;
        SetState("MovementState", new Grounded());
        SetState("AttackState", new Idle());
    }

    public void Update()
    {
        if ( currentMovementState != null )
        {
            currentMovementState.StateUpdate(owner);
        }
    }

    public void FixedUpdate()
    {
        if ( currentMovementState != null )
        {
            currentMovementState.StateFixedUpdate(owner);
        }
    }

    public void SetState(string stateToChange, AState newState)
    {
        switch(stateToChange)
        {
            case "MovementState":
                if ( currentMovementState != null )
                {
                    currentMovementState.StateComplete(owner);
                }
                break;
            case "AttackState":
                if ( currentAttackState != null )
                {
                    currentAttackState.StateComplete(owner);
                }
                break;
        }

        newState.StateStart(owner);

        switch(stateToChange)
        {
            case "MovementState":
                MovementState newMovementState = newState as MovementState;
                currentMovementState = newMovementState;
                break;
            case "AttackState":
                AttackState newAttackState = newState as AttackState;
                currentAttackState = newAttackState;
                break;
        }
    }

    public void APress(InputAction.CallbackContext context)
    {
        currentMovementState.APress(context);
    }

    public void BPress(InputAction.CallbackContext context)
    {
        currentMovementState.BPress(context);
    }

    public void XPress(InputAction.CallbackContext context)
    {
        currentMovementState.XPress(context);
    }

    public void YPress(InputAction.CallbackContext context)
    {
        currentMovementState.YPress(context);
    }

}

