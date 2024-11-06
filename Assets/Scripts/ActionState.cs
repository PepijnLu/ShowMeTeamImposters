using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionState : AState
{
    protected PianoMan character;
    public override void StateStart(GameObject runner)
    {
        if(character == null) character = runner.gameObject.GetComponent<PianoMan>();
    }

    public override void StateComplete(GameObject runner)
    {

    }
}

public class ActionIdle : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);

        character.animator.SetTrigger("WalkFOut");
        character.animator.SetTrigger("WalkBOut");


        character.animator.SetTrigger("Idle");
        character.animator.ResetTrigger("Idle");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
    }
}

public class WalkingBack : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);

        character.animator.ResetTrigger("WalkBOut");
        character.animator.SetTrigger("WalkBIn");
        Debug.Log("ActionState: Enter Walking Back");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
        //character.animator.SetTrigger("WalkBOut");
        Debug.Log("ActionState: Exit Walking Back");
    }
}

public class WalkingForward : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        character.animator.ResetTrigger("WalkFOut");
        character.animator.SetTrigger("WalkFIn");
        Debug.Log("ActionState: Enter Walking Forward");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
        //character.animator.SetTrigger("WalkFOut");
        Debug.Log("ActionState: Exit Walking Forward");
    }
}

public class Running : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        character.animator.ResetTrigger("RunOut");
        character.animator.SetTrigger("RunIn");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
        character.animator.SetTrigger("RunOut");
    }
}

public class DashForward : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        character.animator.SetTrigger("DashForw");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
    }
}

public class DashBack : ActionState
{
    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        character.animator.SetTrigger("DashBack");
    }

    public override void StateComplete(GameObject runner)
    {
        base.StateComplete(runner);
    }
}
