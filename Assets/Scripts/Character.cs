using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private StateMachine stateMachine;
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
