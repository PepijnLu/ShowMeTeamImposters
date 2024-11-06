using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    private int index = 0;
    [SerializeField] List<GameObject> fighters = new();
    private PlayerInputManager manager;

    void Start() 
    {
        manager = GetComponent<PlayerInputManager>();
        index = Random.Range(0, fighters.Count);
        manager.playerPrefab = fighters[index];
    }

    public void SwitchNextSpawnCharacter(PlayerInput input) 
    {
        index = Random.Range(0, fighters.Count);
        manager.playerPrefab = fighters[index];
    }
}
