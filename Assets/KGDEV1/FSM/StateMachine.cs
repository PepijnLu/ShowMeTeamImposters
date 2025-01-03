﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class StateMachine
{
    private GameObject owner;
    public MovementState currentMovementState;
    public AttackState currentAttackState;
    public ActionState currentActionState;

    public StateMachine( GameObject _owner )
    {
        owner = _owner;
        SetState("MovementState", new Grounded());
        SetState("AttackState", new Idle());
        SetState("ActionState", new ActionIdle());
    }

    public void Update()
    {
        if ( currentMovementState != null )
        {
            currentMovementState.StateUpdate(owner);
        }

        if ( currentActionState != null )
        {
            Debug.Log("Current action state: " + currentActionState.GetType().Name);
        }
    }

    public void FixedUpdate()
    {
        if ( currentMovementState != null )
        {
            currentMovementState.StateFixedUpdate(owner);
            currentMovementState.Move();
            Debug.Log("Current state: " + currentMovementState.GetType().Name);
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
            case "ActionState":
                if ( currentActionState != null )
                {
                    currentActionState.StateComplete(owner);
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
            case "ActionState":
                ActionState newActionState = newState as ActionState;
                currentActionState = newActionState;
                break;
        }
    }

    public void APress(InputAction.CallbackContext context)
    {
        //Debug.Log("Current attack state: " + currentAttackState.GetType().Name);
        currentMovementState.APress(context);
        currentAttackState.APress(context);
    }

    public void BPress(InputAction.CallbackContext context)
    {
        currentMovementState.BPress(context);
        currentAttackState.BPress(context);
    }

    public void XPress(InputAction.CallbackContext context)
    {
        currentMovementState.XPress(context);
        currentAttackState.XPress(context);
    }

    public void YPress(InputAction.CallbackContext context)
    {
        currentMovementState.YPress(context);
        currentAttackState.YPress(context);
    }

    public void LeftTriggerPress(InputAction.CallbackContext context, bool pressed)
    {
        currentMovementState.LeftTriggerPress(context, pressed);
        //currentAttackState.LeftTriggerPress(context);
    }

}

