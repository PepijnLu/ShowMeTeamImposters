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
    public virtual void Attack(Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
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
    public override void Attack(Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        character.StartAttackMove(position, startupFrames, activeFrames, recoveryFrames, hitbox, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames, multiHit);
    }

    public override void APress(InputAction.CallbackContext context)
    {
        Debug.Log("Idle state a press");
        if(!character.inHitStun) character.Jab();
    }
}

public class Startup : AttackState
{

}

public class Active : AttackState
{
    
}

public class Recovery : AttackState
{
    
}

