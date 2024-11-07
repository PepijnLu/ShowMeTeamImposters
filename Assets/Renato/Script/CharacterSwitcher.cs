using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    private int index = 0;
    [SerializeField] List<GameObject> fighters = new();
    private PlayerInputManager manager;
    private readonly Queue<int> availableIndexes = new(); 

    void Start() 
    {
        manager = GetComponent<PlayerInputManager>();
        
        // Initialize the queue with all the fighter indices
        for (int i = 0; i < fighters.Count; i++)
        {
            availableIndexes.Enqueue(i);
        }

        // Select the first fighter randomly
        index = availableIndexes.Dequeue();
        manager.playerPrefab = fighters[index];
    }

    public void SwitchNextSpawnCharacter(PlayerInput input) 
    {
        // If there are available characters, select one from the queue
        if (availableIndexes.Count > 0)
        {
            // Get the next available character index from the queue
            index = availableIndexes.Dequeue();
            manager.playerPrefab = fighters[index];
        }
        else
        {
            // If no available characters, reset the queue (if all have been used)
            for (int i = 0; i < fighters.Count; i++)
            {
                availableIndexes.Enqueue(i);
            }

            // Select the next character (again randomly)
            index = availableIndexes.Dequeue();
            manager.playerPrefab = fighters[index];
        }
    }
}
