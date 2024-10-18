using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovementState : AState
{
    protected Rigidbody2D rb;
    protected PianoMan character;
    protected StateMachine sm;
    protected bool isGrounded;
    public override void StateStart(GameObject runner)
    {
        Debug.Log("State Start");
        if(rb == null) rb = runner.gameObject.GetComponent<Rigidbody2D>();
        if(character == null) character = runner.gameObject.GetComponent<PianoMan>();
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        Debug.Log("Character: " + character);
        isGrounded = Physics2D.OverlapCircle(character.groundCheck.position, character.groundCheckRadius, character.groundLayer);
        if(isGrounded) character.stateMachine.SetState("MovementState", new Grounded());
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

public class Airborne : MovementState
{
    private bool jumped = false;

    public override void BPress(InputAction.CallbackContext context)
    {
        DoubleJump();
    }
    public override void XPress(InputAction.CallbackContext context)
    {
        DoubleJump();
    }

    public override void StateFixedUpdate(GameObject runner)
    {
        base.StateFixedUpdate(runner);
        if(isGrounded) character.stateMachine.SetState("MovementState", new Grounded());
    }

    private void DoubleJump()
    {
        if (!jumped) 
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * character.jumpForce, ForceMode2D.Impulse);  // Apply upward force for the jump
        }
        jumped = true;
        Debug.Log("Player Double Jump");
    }
    public override void StateComplete(GameObject runner)
    {
        jumped = false;
    }
}

public class Grounded : MovementState
{
    public override void BPress(InputAction.CallbackContext context)
    {
        Jump();
    }
    public override void XPress(InputAction.CallbackContext context)
    {
        Jump();
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        base.StateFixedUpdate(runner);
        if(!isGrounded) character.stateMachine.SetState("MovementState", new Airborne());
        if(character.moveInput.y < -0.50f) Debug.Log("Should switch to crouching"); character.stateMachine.SetState("MovementState", new Crouching());
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * character.jumpForce, ForceMode2D.Impulse);  // Apply upward force for the jump
        Debug.Log("Player Jump");
    }
}

public class Crouching : MovementState
{

    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        Debug.Log("Switched to crouching");
        character.playerCollider.size = new Vector2(1, 0.5f);
        //character.playerCollider.transform.localScale = new Vector2(1, 0.5f);
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        base.StateFixedUpdate(runner);
        if(!isGrounded) character.stateMachine.SetState("MovementState", new Airborne());
        if(character.moveInput.y > -0.50f) character.stateMachine.SetState("MovementState", new Grounded());
    }

    public override void StateComplete(GameObject runner)
    {
        character.playerCollider.size = character.originalSize;
        //character.playerCollider.transform.localScale = character.originalSize;
    }
}


