using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterCombat characterCombat;
    private float movementSpeed = 7.5f;

    void Awake() 
    {
        characterCombat = new();
    }

    void Start()
    {
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

    private void WalkForward(bool _bool) 
    {
        float direction = _bool ? 1f : -1f; 
        Vector2 move = new(direction * movementSpeed * Time.deltaTime, 0f);
        transform.position += (Vector3)move;
    }
}
