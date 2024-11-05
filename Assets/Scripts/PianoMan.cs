using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PianoMan : MonoBehaviour
{
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private GameObject empty;
    [SerializeField] public Animator animator;
    [SerializeField] private GameObject oppponent, playerCanvas;
    [SerializeField] public float maxWalkSpeed, maxRunSpeed, acceleration, jumpForce, dashCooldown, dashBackSpeed, aerialDriftAcceleration, maxAerialDriftSpeed, jumpSquatFrames;
    [SerializeField] public float groundCheckRadius;
    public StateMachine stateMachine;
    public HitAnim hitAnim;
    public CapsuleCollider2D playerCollider;
    public Vector2 moveInput, originalSize;
    public Slider healthSlider;
    public bool isGrounded;
    public bool inHitStun;
    public bool isRunning, inDashBack;
    public float health;
    private bool readingInputs;
    private Rigidbody2D rb;
    private AttackMove pianoKnee, pianoKick;
    private Dictionary<string, AttackMove> attackMoves = new();
    private Vector2 gizmoPos;
    private float gizmoSize;
    private int frameCountForGroundCheck;
    public bool isFacingLeft, dashOnCooldown, jumpOnCooldown, dashedBackOnInput;

    void Start()
    {   
        healthSlider.maxValue = health;
        healthSlider.value = health;
        stateMachine = new StateMachine(gameObject);
        rb = GetComponent<Rigidbody2D>();
        originalSize = playerCollider.size;
        //originalSize = gameObject.transform.localScale;
        InitializeMoves();
    }

    void Update()
    {
        stateMachine.Update();
        readingInputs = CheckIfReadInputs();
    }

    bool CheckIfReadInputs()
    {
        if(GameManager.instance.inHitStop) return false;
        if(inHitStun) return false;
        return true;
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

    public void Attack(string move)
    {
        AttackMove attackMove = attackMoves[move];
        Vector2 movePos = Vector2.zero;
        if(isFacingLeft) movePos = new Vector2(transform.position.x, transform.position.y) + new Vector2(-attackMove.position.x, attackMove.position.y);
        if(!isFacingLeft) movePos = new Vector2(transform.position.x, transform.position.y) + new Vector2(attackMove.position.x, attackMove.position.y);
        stateMachine.currentAttackState.Attack(move, movePos, attackMove.startupFrames, attackMove.activeFrames, attackMove.recoveryFrames, attackMove.hitbox, attackMove.damage, attackMove.launchAngle, attackMove.launchStrength, attackMove.hitstunFrames, attackMove.hitstopFrames, attackMove.multiHit);
    }

    void FixedUpdate()  // Use FixedUpdate for physics-based movement
    {
        stateMachine.FixedUpdate();
        HandleTurnaround();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        frameCountForGroundCheck = 0;
        Debug.Log("IsGrounded: " + gameObject.name + " " + isGrounded);
    }

    public void HandleTurnaround()
    {
        float posRelativeToOpponent = transform.position.x - oppponent.transform.position.x;
        Debug.Log("rel pos: " + posRelativeToOpponent);
        if(posRelativeToOpponent >= 1)
        {
            if(!isFacingLeft)
            {
                isFacingLeft = true;
                Flip();
            }
        }
        else if(posRelativeToOpponent <= -1)
        {
            if(isFacingLeft)
            {
                isFacingLeft = false;
                Flip();
            }
        }
    }

    private void Flip()
    {
        Debug.Log("Flip left");
        Vector3 playerScale = transform.localScale;
        Vector3 canvasScale = playerCanvas.transform.localScale;
        playerScale.x *= -1;
        canvasScale.x *= -1;
        playerCanvas.transform.localScale = canvasScale;
        transform.localScale = playerScale;
    }

    public void APress(InputAction.CallbackContext context)
    {
        if (context.performed && readingInputs) stateMachine.APress(context);
    }

    public void YPress(InputAction.CallbackContext context)
    {
        if (context.performed && readingInputs) stateMachine.YPress(context);
    }

    public void BPress(InputAction.CallbackContext context)
    {
        if (context.performed && readingInputs) stateMachine.BPress(context);
    }

    public void XPress(InputAction.CallbackContext context)
    {
        if (context.performed && readingInputs) stateMachine.XPress(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && readingInputs)
        {
            moveInput = context.ReadValue<Vector2>();  // Read the Vector2 value (X and Y)
            Debug.Log("Joystick movement: " + moveInput);
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    public void StartAttackMove(string moveName, Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        StartCoroutine(AttackMove(moveName, position, startupFrames, activeFrames, recoveryFrames, hitbox, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames, false));
    }

    public IEnumerator AttackMove(string moveName, Vector2 position, float startupFrames, float activeFrames, float recoveryFrames, float hitbox, float damage, Vector2 launchAngle, float launchStrength, float hitstunFrames, float hitstopFrames, bool multiHit)
    {
        // GameManager.instance.audioSource.Play();

        Collider2D[] hitColliders;
        List<string> hitCharacters = new();
        GameObject movePos = Instantiate(empty, position, Quaternion.identity);
        movePos.transform.SetParent(gameObject.transform);

        //startup logic
        //animator.SetTrigger("Kick");
        Debug.Log("Move name = " + moveName);
        animator.SetTrigger(moveName);
        stateMachine.SetState("AttackState", new Startup());
        for(int i = 0; i < startupFrames; i++) 
        {
            //during startup logic
            Debug.Log("WAIT A FRAME");
            yield return new WaitForFixedUpdate();
        }

        //become active logic
        stateMachine.SetState("AttackState", new Active());
        for(int i = 0; i < activeFrames; i++) 
        {
            //Check for collision
            hitColliders = Physics2D.OverlapCircleAll(movePos.transform.position, hitbox);
            gizmoPos = movePos.transform.position;
            gizmoSize = hitbox;

            if (hitColliders.Length > 0)
            {
                foreach (Collider2D hitCollider in hitColliders)
                {
                    Debug.Log("Collision detected with: " + gameObject.name + " , " + hitCollider.gameObject.name);

                    if(hitCollider.TryGetComponent(out PianoMan component))
                    {
                        if ((!hitCharacters.Contains(hitCollider.gameObject.name) || multiHit) && (hitCollider.gameObject.name != gameObject.name))
                        {
                            StartCoroutine(HitCharacter(movePos.transform.position, hitCollider.gameObject, damage, launchAngle, launchStrength, hitstunFrames, hitstopFrames));
                            hitCharacters.Add(hitCollider.gameObject.name);
                            Debug.Log("111" + hitCharacters[0]);
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        //start recovery logic
        empty.SetActive(false);
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

        hitAnim.transform.position = position;
        hitAnim.spriteRenderer.enabled = true;
        hitAnim.animator.SetTrigger("1Hit");

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
        //float direction = character.transform.position.x - position.x;

        launchAngle.Normalize();
        if(isFacingLeft)
        {
            launchAngle.x = -launchAngle.x;
        }

        if(character.TryGetComponent<PianoMan>(out PianoMan component))
        {
            component.TakeDamage(damage);
            //Handle hitstop
            Coroutine hitstopRoutine = StartCoroutine(GameManager.instance.HandleHitStop(hitstopFrames, hitstopBeats));
            yield return hitstopRoutine; 
            StartCoroutine(HandleHitStun(component, hitstunFrames));
        }
        character.GetComponent<Rigidbody2D>().AddForce(launchAngle * launchStrength);

        yield return null;
    }

    private IEnumerator HandleHitStun(PianoMan character, float hitstunFrames)
    {
        character.inHitStun = true;
        for(int i = 0; i < hitstunFrames; i++) yield return new WaitForFixedUpdate(); 
        character.inHitStun = false;
    }
    
    private IEnumerator Jump()
    {
        animator.SetTrigger("Jump");
        jumpOnCooldown = true;
        for(int i = 0; i < jumpSquatFrames; i++) yield return new WaitForFixedUpdate();
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);  // Apply upward force for the jump
        Debug.Log("Player Jump");
        StartJumpCooldown();
        yield return null;
    }

    public void StartJump()
    {
        StartCoroutine(Jump());
    }

    void OnDrawGizmos()
    {
        // Set the color for the gizmo
        Gizmos.color = Color.red;

        // Draw the wireframe circle at the specified position with the specified radius
        Gizmos.DrawWireSphere(gizmoPos, gizmoSize);
    }

    void InitializeMoves()
    {
        pianoKnee = new()
        {
            position = new Vector2(1f, 1.25f),
            startupFrames = 10,
            activeFrames = 9,
            recoveryFrames = 25,
            hitbox = 0.25f,
            damage = 1, 
            launchAngle = new Vector2(0.5f, 0.5f),
            launchStrength = 300,
            hitstunFrames = 15,
            //5 is perfect
            hitstopFrames = 10,
            multiHit = false
        };

        pianoKick = new()
        {
            position = new Vector2(2f, 1.25f),
            startupFrames = 10,
            activeFrames = 9,
            recoveryFrames = 25,
            hitbox = 0.25f,
            damage = 1, 
            launchAngle = new Vector2(0.5f, 0.5f),
            launchStrength = 300,
            hitstunFrames = 15,
            //5 is perfect
            hitstopFrames = 10,
            multiHit = false
        };

        attackMoves.Add("pianoKnee", pianoKnee);
        attackMoves.Add("pianoKick", pianoKick);
    }

    public void StartDashCooldown()
    {
        StartCoroutine(DashCooldown());
    }

    public void StartJumpCooldown()
    {
        StartCoroutine(JumpCooldown());
    }

    public IEnumerator JumpCooldown()
    {
        for(int i = 0; i < 5; i++) yield return new WaitForFixedUpdate();
        jumpOnCooldown = false; 
    }

    public IEnumerator DashCooldown()
    {
        for(int i = 0; i < dashCooldown; i++) yield return new WaitForFixedUpdate();
        dashOnCooldown = false;
        inDashBack = false;
    }
}
