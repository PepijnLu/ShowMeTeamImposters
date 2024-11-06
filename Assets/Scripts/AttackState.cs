using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackState : AState
{
    protected PianoMan character;

    public override void StateStart(GameObject runner)
    {
        if(character == null) character = runner.gameObject.GetComponent<PianoMan>();
    }
    public virtual void Attack(string moveName, Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {

    }

    public virtual void APress(InputAction.CallbackContext context)
    {
        Debug.Log("A button pressed");
    }

    public virtual void YPress(InputAction.CallbackContext context)
    {
        Debug.Log("Y button pressed");
    }

    public virtual void BPress(InputAction.CallbackContext context)
    {
        Debug.Log("B button pressed");
    }

    public virtual void XPress(InputAction.CallbackContext context)
    {
        Debug.Log("X button pressed");
    }
}

public class Idle : AttackState
{
    public override void Attack(string moveName, Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        character.StartAttackMove(moveName, position, startupFrames, activeFrames, recoveryFrames, hitbox, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames, multiHit);
    }

    public override void APress(InputAction.CallbackContext context)
    {
        Debug.Log("Idle state A press");
        if(!character.inHitStun) 
        {
            if(character.stateMachine.currentMovementState.GetType().Name == "Grounded")
            {
                if(character.gameObject.name == "PianoMan") character.Attack("pianoKnee");
                if(character.gameObject.name == "GuitarLady") character.Attack("guitarPunch1");
            }
            if(character.stateMachine.currentMovementState.GetType().Name == "Crouching")
            {
                //if(character.gameObject.name == "PianoMan") character.Attack("pianoKnee");
                if(character.gameObject.name == "GuitarLady") character.Attack("guitarCrouchPunch");
            }
            if(character.stateMachine.currentMovementState.GetType().Name == "Airborne")
            {
                //if(character.gameObject.name == "PianoMan") character.Attack("pianoKnee");
                if(character.gameObject.name == "GuitarLady") character.Attack("guitarAerial");
            }
        }
    }

    public override void YPress(InputAction.CallbackContext context)
    {
        Debug.Log("Idle state Y press");
        //if(!character.inHitStun) character.Attack("pianoKick");
    }
}

public class Startup : AttackState
{
    public override void APress(InputAction.CallbackContext context)
    {
        if(character.activeMove.moveName != "nullMove" && !character.moveBuffered)
        {
            Debug.Log("Atm active move: " + character.activeMove.moveName);
            if(character.activeMove.moveName == "pianoKnee") character.bufferedMove = character.attackMoves["pianoKick"]; character.moveBuffered = true;
            if(character.activeMove.moveName == "guitarPunch1") character.bufferedMove = character.attackMoves["guitarPunch2"]; character.moveBuffered = true;
        }
    }
}

public class Active : AttackState
{
    public override void APress(InputAction.CallbackContext context)
    {
        if(character.activeMove.moveName != "nullMove" && !character.moveBuffered)
        {
            Debug.Log("Startup A Press");
            if(character.activeMove.moveName == "pianoKnee") character.bufferedMove = character.attackMoves["pianoKick"]; character.moveBuffered = true;
            if(character.activeMove.moveName == "guitarPunch1") character.bufferedMove = character.attackMoves["guitarPunch2"]; character.moveBuffered = true;
        }
    }
}

public class Recovery : AttackState
{
    public override void APress(InputAction.CallbackContext context)
    {
        if(character.activeMove.moveName != "nullMove" && !character.moveBuffered)
        {
            Debug.Log("Startup A Press");
            if(character.activeMove.moveName == "pianoKnee") character.bufferedMove = character.attackMoves["pianoKick"]; character.moveBuffered = true;
            if(character.activeMove.moveName == "guitarPunch1") character.bufferedMove = character.attackMoves["guitarPunch2"]; character.moveBuffered = true;
        }
    }  
}

