using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PianoMan : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    public StateMachine stateMachine;
    public CapsuleCollider2D playerCollider;
    public Vector2 moveInput, originalSize;
    [SerializeField] public float moveSpeed, jumpForce;
    [SerializeField] public float groundCheckRadius;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {   
        stateMachine = new StateMachine(gameObject);
        rb = GetComponent<Rigidbody2D>();
        originalSize = playerCollider.size;
        //originalSize = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()  // Use FixedUpdate for physics-based movement
    {
        stateMachine.FixedUpdate();

        // Convert the 2D input vector into a 3D vector for applying force
        Vector2 force = new Vector2(moveInput.x, moveInput.y) * moveSpeed;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log("IsGrounded: " + isGrounded);

        // Apply force to the Rigidbody (keeping it in world space)
        rb.AddForce(force);
    }

    public void APress(InputAction.CallbackContext context)
    {
        if (context.performed) stateMachine.APress(context);
    }

    public void YPress(InputAction.CallbackContext context)
    {
        if (context.performed) stateMachine.YPress(context);
    }

    public void BPress(InputAction.CallbackContext context)
    {
        if (context.performed) stateMachine.BPress(context);
    }

    public void XPress(InputAction.CallbackContext context)
    {
        if (context.performed) stateMachine.XPress(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();  // Read the Vector2 value (X and Y)
            Debug.Log("Joystick movement: " + moveInput);
        }
    }
}
