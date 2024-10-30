using System;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int HP = 400; // (100 per bar) (can do something like increased damage intake at each new bar layer)
    private int damage = 3;
    private float layerHP = 100f; // Initialize with full layer capacity

    // Key press
    public KeyCode key = KeyCode.P; // Key to detect
    public float requiredTimeFrame = 3.0f; // Exact time required between presses in seconds
    public float tolerance = 0.2f; // Tolerance window in seconds

    public float firstPressTime = -1f; // Tracks the time of the first key press
    private bool awaitingSecondPress = false;

    [Header("UI")]
    public PlayerUI playerUI;

    void Start() 
    {
        playerUI.InstantiatePlayerUI(UIManager.instance.UIPrefab, UIManager.instance.player1_UI);
        // playerUI.CustomStart();
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.P)) 
        // {
        //     TakeDamage(damage);
        // }

        KeyPress();
    }

    private void KeyPress()
    {
        if (Input.GetKeyDown(key))
        {
            if (!awaitingSecondPress)
            {
                // First press starts the timer
                firstPressTime = Time.time;
                awaitingSecondPress = true;
                Debug.Log("First key press detected. Waiting for second press...");

                TakeDamage(damage);

            }
            else
            {
                // Second press checks if it falls within the acceptable range
                float timeSinceFirstPress = Time.time - firstPressTime;
                if (Mathf.Abs(timeSinceFirstPress - requiredTimeFrame) <= tolerance)
                {
                    Debug.Log("Rhythm met! Perfect timing.");
                    TakeDamage(damage);
                }
                else
                {
                    Debug.Log("Missed the rhythm. Try again.");
                }
                
                // Reset the timing condition after the second press
                awaitingSecondPress = false;
                firstPressTime = -1f;
            }
        }
        
        // Reset if no second press occurs within a reasonable time (e.g., 5 seconds)
        if (awaitingSecondPress && Time.time - firstPressTime > requiredTimeFrame + tolerance + 2f)
        {
            Debug.Log("Too slow, start over.");
            awaitingSecondPress = false;
            firstPressTime = -1f;
        }
    }

    private void TakeDamage(int incomingDamage) 
    {
        HP -= incomingDamage;
        HP = Mathf.Max(HP, 0);

        layerHP -= incomingDamage;
        layerHP = MathF.Max(layerHP, 0);

        Debug.Log($"Layer HP: {layerHP} + Player HP: {HP}");

        playerUI.UpdateHealthBar(HP, ref layerHP);

        if(HP <= 0) 
        {
            HP = 0;
            Debug.Log("Death...");
            // RestartPlayMode();
        }
    }

    private void RestartPlayMode() 
    {
        if(EditorApplication.isPlaying) 
        {
            EditorApplication.isPlaying = false;
        }

        EditorApplication.isPlaying = true;
    }
}
