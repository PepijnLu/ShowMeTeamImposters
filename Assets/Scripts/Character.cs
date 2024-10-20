using UnityEngine;

public class Character : MonoBehaviour
{
    private StateMachine stateMachine;

    void Start()
    {
        stateMachine = new StateMachine(gameObject);
    }

    void Update()
    {
        
    }
}
