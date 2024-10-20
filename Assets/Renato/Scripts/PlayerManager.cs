using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CharacterCombat characterCombat;
    public CharacterStats stats;
    public bool isAttacking = false;

    void Awake() 
    {
        characterCombat = new(this);
    }

    void Start()
    {
        name = stats.Name;
        characterCombat.animator.Start(transform);
    }

    void Update()
    {
        InputManagement();
    }
    
    private void InputManagement() 
    {
        // Movement
        if(Input.GetKey(KeyCode.RightArrow)) 
        {
            WalkForward(true);
            characterCombat.animator.WalkForwardAnim();
        }
        else if(Input.GetKey(KeyCode.LeftArrow)) 
        {
            WalkForward(false);
            characterCombat.animator.WalkBackwardAnim();
        }

        // Attack
        if(Input.GetKeyDown(KeyCode.Q)) 
        {
            characterCombat.Punch();
        }
    }

    public void ResetAttack() 
    {
        isAttacking = false;
    }

    private void WalkForward(bool _bool) 
    {
        float direction = _bool ? 1f : -1f; 
        Vector2 move = new(direction * stats.walkSpeed.GetValue() * Time.deltaTime, 0f);
        transform.position += (Vector3)move;
    }
}
