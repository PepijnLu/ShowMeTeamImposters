using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlayerInputManager : MonoBehaviour
{
    public GameObject[] characterPrefabs;  // Array for different character prefabs/skins
    public Transform[] spawnPoints;        // Array of spawn points for players
    private int playerCount = 0;            // Track the number of players spawned

    private void Start()
    {
        // Ensure we subscribe to the onPlayerJoined event
        if (PlayerInputManager.instance != null)
        {
            // No longer automatically joining players, but you can use this to manage other player inputs
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
        else
        {
            Debug.LogError("PlayerInputManager instance is missing!");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the PlayerInputManager's onPlayerJoined event
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
    }

    // Custom method to manually join a player and instantiate their character prefab
    public void JoinPlayer(PlayerInput playerInput)
    {
        if (playerCount < characterPrefabs.Length && playerCount < spawnPoints.Length)
        {
            // Instantiate the correct character at the spawn point
            Transform spawnPoint = spawnPoints[playerCount];
            GameObject selectedPrefab = characterPrefabs[playerCount];

            // Instantiate the character model at the player's spawn point
            GameObject characterModel = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

            // Set the character model to be a child of the PlayerInput object (this links the input to the character)
            characterModel.transform.parent = playerInput.transform;

            // Position the player correctly
            playerInput.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            playerCount++; // Increase the count of players
        }
        else
        {
            Debug.LogWarning("Maximum number of players reached or no available spawn points.");
        }
    }

    // Handle input (for example, a button press to trigger player joining)
    private void Update()
    {
        // Detect button presses or actions here to join players manually
        if (Keyboard.current.spaceKey.wasPressedThisFrame && playerCount < characterPrefabs.Length)
        {
            // Trigger the JoinPlayer method manually, simulate a player joining action
            PlayerInputManager.instance.JoinPlayer(playerCount);
        }
    }

    // OnPlayerJoined could be used if you need to do something when the player actually joins
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.playerIndex);
    }
}
