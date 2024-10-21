using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PianoMan : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    public StateMachine stateMachine;
    public CapsuleCollider2D playerCollider;
    public Vector2 moveInput, originalSize;
    private bool inHitStun;
    [SerializeField] public float moveSpeed, jumpForce;
    [SerializeField] public float groundCheckRadius;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    public float health;
    public Slider healthSlider;

    private Vector2 gizmoPos;
    private float gizmoSize;

    // Start is called before the first frame update
    void Start()
    {   
        healthSlider.maxValue = health;
        healthSlider.value = health;
        stateMachine = new StateMachine(gameObject);
        rb = GetComponent<Rigidbody2D>();
        originalSize = playerCollider.size;
        //originalSize = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        //TestInputs
        if(!inHitStun)
        {
            if(Input.GetKeyDown(KeyCode.Space) && gameObject.name == "PianoMan")
            {
                Debug.Log("Space input");
                InitiateAttack();
            }

            if(Input.GetKeyDown(KeyCode.Return) && gameObject.name == "CPU")
            {
                Vector2 movePosition = new Vector2(transform.position.x, transform.position.y) + new Vector2(2, 0);
                stateMachine.currentAttackState.Attack(movePosition, 20, 60, 60, 0.5f, 15, new Vector2(1, 0), 250, 30, 30, false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health;

        if(health <= 0)
        {
            health = 0;
            //Die logic
        }
    }

    public void InitiateAttack()
    {
        Vector2 movePosition = new Vector2(transform.position.x, transform.position.y) + new Vector2(2, 0);
        stateMachine.currentAttackState.Attack(movePosition, 0, 60, 60, 0.5f, 1, new Vector2(1, 0), 250, 120, 30, false);
    }

    void FixedUpdate()  // Use FixedUpdate for physics-based movement
    {
        stateMachine.FixedUpdate();

        if(!inHitStun)
        {
            // Convert the 2D input vector into a 3D vector for applying force
            Vector2 force = new Vector2(moveInput.x, moveInput.y) * moveSpeed;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            Debug.Log("IsGrounded: " + isGrounded);

            // Apply force to the Rigidbody (keeping it in world space)
            rb.AddForce(force);
        }
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

    public void StartAttackMove(Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        StartCoroutine(AttackMove(position, startupFrames, activeFrames, recoveryFrames, hitbox, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames, false));
    }

    public IEnumerator AttackMove(Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        // GameManager.instance.audioSource.Play();
        Collider2D[] hitColliders;
        List<string> hitCharacters = new();

        //startup logic
        stateMachine.SetState("AttackState", new Startup());
        for(int i = 0; i < startupFrames; i++) 
        {
            //during startup logic
            Debug.Log("WAIT A FRAME");
            yield return new WaitForFixedUpdate();
        }
        //become active logic
        stateMachine.SetState("AttackState", new Active());
        //GameObject circle = Instantiate(gizmoCircle, position, Quaternion.identity);
        for(int i = 0; i < activeFrames; i++) 
        {
            //Check for collision
            hitColliders = Physics2D.OverlapCircleAll(position, hitbox);
            gizmoPos = position;
            gizmoSize = hitbox;

            if (hitColliders.Length > 0)
            {
                foreach (Collider2D hitCollider in hitColliders)
                {
                    Debug.Log("Collision detected with: " + hitCollider.gameObject.name);
                    if ((!hitCharacters.Contains(hitCollider.gameObject.name) || multiHit) && (!hitCollider.gameObject != gameObject))
                    {
                        StartCoroutine(HitCharacter(position, hitCollider.gameObject, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames));
                        hitCharacters.Add(hitCollider.gameObject.name);
                        Debug.Log("111" + hitCharacters[0]);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
        //start recovery logic
        stateMachine.SetState("AttackState", new Recovery());
        gizmoSize = 0;
        gizmoPos = new Vector2(0, 0);
        for(int i = 0; i < recoveryFrames; i++) 
        {
            //during recovery logic
            yield return new WaitForFixedUpdate();
        }
        //end recovery logic
        stateMachine.SetState("AttackState", new Idle());
        yield return null;
    }

    IEnumerator HitCharacter(Vector2 position, GameObject character, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames)
    {
        GameManager.instance.snare.Play();
        Debug.Log($" HitDetection: {character.name} hit");
        string accuracy = GameManager.instance.GetAccuracyOnBeat();

        float damageMult = 0;
        float launchStrengthMult = 0;
        float hitstunFramesMult = 0;
        float hitstopFramesMult = 0;
        float hitstopBeats = 0;

        switch(accuracy)
        {
            case "Perfect":
                Debug.Log("Hit: Perfect");
                damageMult = 3;
                launchStrengthMult = 3;
                hitstunFramesMult = 3;
                hitstopFramesMult = 3;
                hitstopBeats = 2;
                break;
            case "OK":
                Debug.Log("Hit: OK");
                damageMult = 1.5f;
                launchStrengthMult = 1.5f;
                hitstunFramesMult = 1.5f;
                hitstopFramesMult = 1.5f;
                hitstopBeats = 1;
                break;
            case "Bad":
                Debug.Log("Hit: Bad");
                damageMult = .5f;
                launchStrengthMult = .5f;
                hitstunFramesMult = .5f;
                hitstopFramesMult = .5f;
                hitstopBeats = .5f;
                break;
        }

        damage *= damageMult;
        launchStrength *= launchStrengthMult;
        hitstunFrames *= hitstunFramesMult;
        hitstopFrames *= hitstopFramesMult;

        // Step 1: Calculate the direction from object1 to object2
        Vector2 direction = new Vector2(character.transform.position.x, character.transform.position.x) - position;

        // Step 2: Get the angle (in degrees) from the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Step 3: Normalize the direction vector and move object2 forward
        direction.Normalize();
        direction += launchAngle.normalized;
        direction.Normalize();

        if(character.TryGetComponent<PianoMan>(out PianoMan component))
        {
            component.TakeDamage(damage);
            //Handle hitstop
            Rigidbody2D rb2D = component.GetComponent<Rigidbody2D>();
            rb2D.velocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            Coroutine hitstopRoutine = StartCoroutine(GameManager.instance.HandleHitStop(hitstopFrames, hitstopBeats));
            yield return hitstopRoutine; 
            StartCoroutine(HandleHitStun(component, hitstunFrames));
        }
        character.GetComponent<Rigidbody2D>().AddForce(direction * launchStrength);

        yield return null;
    }

    private IEnumerator HandleHitStun(PianoMan character, float hitstunFrames)
    {
        character.inHitStun = true;
        for(int i = 0; i < hitstunFrames; i++) yield return new WaitForFixedUpdate(); 
        character.inHitStun = false;
    }

    void OnDrawGizmos()
    {
        // Set the color for the gizmo
        Gizmos.color = Color.red;

        // Draw the wireframe circle at the specified position with the specified radius
        Gizmos.DrawWireSphere(gizmoPos, gizmoSize);
    }
}
