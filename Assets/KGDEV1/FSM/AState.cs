using UnityEngine;

public abstract class AState
{
    public virtual void StateStart( GameObject runner ){}
    public virtual void StateUpdate( GameObject runner ){}
    public virtual void StateFixedUpdate( GameObject runner ){}
    public virtual void StateComplete( GameObject runner ){}
}
