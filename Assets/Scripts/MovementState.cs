using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementState : AState
{
    protected Vector2 moveInput;
    protected Rigidbody2D rb;
    protected PianoMan character;
    protected StateMachine sm;
    public override void StateStart(GameObject runner)
    {
        Debug.Log("State Start");
        if(rb == null) rb = runner.gameObject.GetComponent<Rigidbody2D>();
        if(character == null) character = runner.gameObject.GetComponent<PianoMan>();
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        Debug.Log("Character: " + character);
        if(character.isGrounded) character.stateMachine.SetState("MovementState", new Grounded());
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

    public virtual void Move()
    {
        moveInput = character.moveInput;
    }
}

public class Airborne : MovementState
{
    private bool jumped = false;
    private bool animationTriggered = false;

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
        if(character.isGrounded) character.stateMachine.SetState("MovementState", new Grounded());

        if(rb.velocity.y <= -0.1 && !animationTriggered)
        {
            character.animator.SetTrigger("Descent");
            animationTriggered = true;
        }
    }

    public override void Move()
    {
        if(character.inHitStun) return;
        //if(character.gameObject.name != "Character") return;

        base.Move();
        float moveInputNormalized = 0;
        if(moveInput.x > 0.1f) moveInputNormalized = 1;
        else if(moveInput.x < -0.1f) moveInputNormalized = -1;

        Vector2 desiredForce = new Vector2(moveInputNormalized * character.aerialDriftAcceleration, 0);
        Vector2 predictedVelocity = new Vector2(rb.velocity.x + (desiredForce.x * Time.fixedDeltaTime / rb.mass), 0);
        float maxToCheck = character.maxAerialDriftSpeed;
        Vector2 clampedForce;
        
        if (predictedVelocity.magnitude > maxToCheck)
        {
            clampedForce = (predictedVelocity.normalized * maxToCheck - rb.velocity) * rb.mass / Time.fixedDeltaTime;
            Vector2 forceToAdd = new Vector2(clampedForce.x, 0);
            rb.AddForce(forceToAdd);
        }
        else
        {
            // Apply the full force if within limits
            rb.AddForce(desiredForce);
        }
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
    public override void Move()
    {
        if(character.inHitStun) return;
        if(character.inDashBack) return;
        
        base.Move();
        float requiredForce = 0f;
        Debug.Log("X Move Input: " + moveInput.x);

        //Facing left
        if(character.isFacingLeft)
        {
            //Run left
            if(moveInput.x <= -0.8f)
            {
                if(!character.isRunning) character.animator.SetTrigger("DashForw");
                character.isRunning = true;
                //requiredForce = -(character.maxRunSpeed - rb.velocity.x) * rb.mass;
                requiredForce = -character.dashBackSpeed;
                character.dashedBackOnInput = false;
            }
            //Dash right
            else if(moveInput.x >= 0.8f)
            {
                if(!character.dashedBackOnInput)
                {
                    DashBack();
                    character.dashedBackOnInput = true;
                    return;
                }
                else
                {
                    requiredForce = character.acceleration;
                    character.isRunning = false;
                }
            }
            //Walk left
            else if(moveInput.x < -0.1)
            {
                requiredForce = -character.acceleration;
                character.isRunning = false;
                character.dashedBackOnInput = false;
            }
            //Walk right
            else if(moveInput.x > 0.1)
            {
                requiredForce = character.acceleration;
                character.isRunning = false;
                character.dashedBackOnInput = false;
            }
        }
        //Facing right
        else
        {
            //Run right
            if(moveInput.x >= 0.8f)
            {
                Debug.Log("character.isRunning: " + character.isRunning);
                if(!character.isRunning) character.animator.SetTrigger("DashForw");
                character.isRunning = true;
                //requiredForce = (character.maxRunSpeed - rb.velocity.x) * rb.mass;
                requiredForce = character.dashBackSpeed;
                character.dashedBackOnInput = false;
            }
            //Dash left
            else if(moveInput.x <= -0.8f)
            {
                if(!character.dashedBackOnInput)
                {
                    DashBack();
                    character.dashedBackOnInput = true;
                    return;
                }
                else
                {
                    requiredForce = -character.acceleration;
                    character.isRunning = false;
                }
            }
            //Walk left
            else if(moveInput.x < -0.1)
            {
                requiredForce = -character.acceleration;
                character.isRunning = false;
                character.dashedBackOnInput = false;
            }
            //Walk right
            else if(moveInput.x > 0.1)
            {
                requiredForce = character.acceleration;
                character.isRunning = false;
                character.dashedBackOnInput = false;
            }
        }

        Debug.Log("Required Force: " + requiredForce);


        Vector2 desiredForce = new Vector2(requiredForce, 0);
        Vector2 predictedVelocity = new Vector2(rb.velocity.x + (desiredForce.x * Time.fixedDeltaTime / rb.mass), 0);
        float maxToCheck;
        Vector2 clampedForce;

        if(character.isRunning) maxToCheck = character.maxRunSpeed;
        else maxToCheck = character.maxWalkSpeed;
        
        if (predictedVelocity.magnitude > maxToCheck)
        {
            clampedForce = (predictedVelocity.normalized * maxToCheck - rb.velocity) * rb.mass / Time.fixedDeltaTime;
            Vector2 forceToAdd = new Vector2(clampedForce.x, 0);
            rb.AddForce(forceToAdd);
        }
        else
        {
            // Apply the full force if within limits
            rb.AddForce(desiredForce);
        }

        Debug.Log("X Velocity: " + rb.velocity.x);
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        base.StateFixedUpdate(runner);
        if(!character.isGrounded) 
        {
            character.stateMachine.SetState("MovementState", new Airborne());
            Debug.Log("Switched to Airborne");
        }
        Debug.Log("moveInput Y: " + character.moveInput.y);
        //if(character.moveInput.y < -0.50f) Debug.Log("Should switch to crouching"); character.stateMachine.SetState("MovementState", new Crouching());
    }

    private void Jump()
    {
        if(!character.jumpOnCooldown)
        {
            character.StartJump();
        }
    }

    private void DashBack()
    {
        if(!character.dashOnCooldown)
        {
            character.inDashBack = true;
            character.animator.SetTrigger("DashBack");
            character.dashOnCooldown = true;
            Debug.Log("Dash back");
            rb.velocity = new Vector2(0, rb.velocity.y);
            Vector2 movement = Vector2.zero;
            if(!character.isFacingLeft) movement = new Vector2((-character.dashBackSpeed - rb.velocity.x) * rb.mass, 0);
            if(character.isFacingLeft) movement = new Vector2((character.dashBackSpeed - rb.velocity.x) * rb.mass, 0);
            rb.AddForce(movement);
            character.StartDashCooldown();
        }
    }
}

public class Crouching : MovementState
{

    public override void StateStart(GameObject runner)
    {
        base.StateStart(runner);
        Debug.Log("Switched to crouching");
        //character.playerCollider.size = new Vector2(1, 0.5f);
        //character.playerCollider.transform.localScale = new Vector2(1, 0.5f);
    }
    public override void StateFixedUpdate(GameObject runner)
    {
        base.StateFixedUpdate(runner);
        if(!character.isGrounded) character.stateMachine.SetState("MovementState", new Airborne());
        if(character.moveInput.y > -0.50f) character.stateMachine.SetState("MovementState", new Grounded());
    }

    public override void StateComplete(GameObject runner)
    {
        //character.playerCollider.size = character.originalSize;
        //character.playerCollider.transform.localScale = character.originalSize;
    }
}


