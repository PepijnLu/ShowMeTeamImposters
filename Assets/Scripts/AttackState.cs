using UnityEngine;

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
}

public class Idle : AttackState
{
    public override void Attack(Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        character.StartAttackMove(position, startupFrames, activeFrames, recoveryFrames, hitbox, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames, multiHit);
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

