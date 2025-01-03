using System;
using UnityEngine;

[Serializable]
public class ComboManagement 
{
    [SerializeField] private KeyCode key; // P
    [SerializeField] private float requiredTimeFrame; // 1
    [SerializeField] private float tolerance; // .2
    [SerializeField] private float lastPressTime; // -1
    [SerializeField] private int comboCounter; // 0

    [SerializeField] private int hitCounter = 0; // Tracks hits for instantiating combo notes
    [SerializeField] private int comboNoteCounter = 0; // Tracks combo notes for activating power notes

    [SerializeField] private Transform instantiatePoint;


    public void CheckComboKeyPress(PlayerUIManagement playerUIManagement, int HP, float layerHP, int damage)
    {
        if (lastPressTime < 0)
        {
            // First press initializes the timer and starts the combo
            lastPressTime = Time.time;
            TakeDamage(playerUIManagement, HP, layerHP, damage);
            return; // Exit to avoid further checks on first press
        }
        
        // Calculate time since last successful press
        float timeSinceLastPress = Time.time - lastPressTime;

        if (Mathf.Abs(timeSinceLastPress - requiredTimeFrame) <= tolerance)
        {
            // Successful timing: increase hit counter
            hitCounter++;
            lastPressTime = Time.time;
            Debug.Log($"Successful rhythm hit! Hit counter: {hitCounter}");

            // Increase damage output
            TakeDamage(playerUIManagement, HP, layerHP, damage);

            // Check if we should increase combo count
            if (hitCounter == 2)
            {
                comboCounter++; // Increment combo count
                hitCounter = 0; // Reset hit counter after two successful hits

                // Spawn combo note for every two combo hits
                if (comboCounter % 2 == 0)
                {
                    playerUIManagement.InstantiateComboUIElement(ref comboCounter, ref lastPressTime);

                    // Check for activating a power note
                    comboNoteCounter++; // Increment combo note counter

                    if (comboNoteCounter % 2 == 0)
                    {
                        playerUIManagement.UpdateComboBar(ref playerUIManagement.powerNoteBarIndex, ref playerUIManagement.powerNoteIndex, playerUIManagement.totalPowerNotesActiveAtOnce, null, ref comboCounter, ref lastPressTime);
                    }
                }
            }
        }
        else
        {
            // Reset if timing fails
            playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
        }
        
        // Reset if too much time has passed without a successful hit
        if (lastPressTime >= 0 && Time.time - lastPressTime > requiredTimeFrame + tolerance + 2f)
        {
            playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
        }
    }

    public void TakeDamage(PlayerUIManagement playerUIManagement, int HP, float layerHP, int incomingDamage) 
    {
        HP -= incomingDamage;
        HP = Mathf.Max(HP, 0);

        layerHP -= incomingDamage;
        layerHP = MathF.Max(layerHP, 0);

        // Debug.Log($"Layer HP: {layerHP} + Player HP: {HP}");

        playerUIManagement.UpdateHealthBar(HP, ref layerHP);

        if(HP <= 0) 
        {
            HP = 0;
            Debug.Log("Death...");
            // RestartPlayMode();
        }
    }

    // public void CheckComboKeyPress(PlayerManager playerManager, int damage)
    // {
    //     if (Input.GetKeyDown(key))
    //     {
    //         if (lastPressTime < 0)
    //         {
    //             // First press initializes the timer and starts the combo
    //             lastPressTime = Time.time;
    //             playerManager.TakeDamage(damage);
    //             return; // Exit to avoid further checks on first press
    //         }
            
    //         // Calculate time since last successful press
    //         float timeSinceLastPress = Time.time - lastPressTime;

    //         if (Mathf.Abs(timeSinceLastPress - requiredTimeFrame) <= tolerance)
    //         {
    //             // Successful timing: increase hit counter
    //             hitCounter++;
    //             lastPressTime = Time.time;
    //             Debug.Log($"Successful rhythm hit! Hit counter: {hitCounter}");

    //             // Increase damage output
    //             playerManager.TakeDamage(damage);

    //             // Check if we should increase combo count
    //             if (hitCounter == 2)
    //             {
    //                 comboCounter++; // Increment combo count
    //                 hitCounter = 0; // Reset hit counter after two successful hits

    //                 // Spawn combo note for every two combo hits
    //                 if (comboCounter % 2 == 0)
    //                 {
    //                     playerManager.playerUIManagement.InstantiateComboUIElement(ref comboCounter, ref lastPressTime);

    //                     // Check for activating a power note
    //                     comboNoteCounter++; // Increment combo note counter

    //                     if (comboNoteCounter % 2 == 0)
    //                     {
    //                         playerManager.playerUIManagement.UpdateComboBar(ref playerManager.playerUIManagement.powerNoteBarIndex, ref playerManager.playerUIManagement.powerNoteIndex, playerManager.playerUIManagement.totalPowerNotesActiveAtOnce, null, ref comboCounter, ref lastPressTime);
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // Reset if timing fails
    //             playerManager.playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
    //         }
    //     }

    //     // Reset if too much time has passed without a successful hit
    //     if (lastPressTime >= 0 && Time.time - lastPressTime > requiredTimeFrame + tolerance + 2f)
    //     {
    //         playerManager.playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
    //     }
    // }

    // public void CheckComboKeyPress(PlayerManager playerManager, int damage)
    // {
    //     if (Input.GetKeyDown(key))
    //     {
    //         if (lastPressTime < 0)
    //         {
    //             // First press initializes the timer and starts the combo
    //             lastPressTime = Time.time;
    //             playerManager.TakeDamage(damage);
    //         }
    //         else
    //         {
    //             // Calculate time since last successful press
    //             float timeSinceLastPress = Time.time - lastPressTime;
                
    //             if (Mathf.Abs(timeSinceLastPress - requiredTimeFrame) <= tolerance)
    //             {
    //                 // Successful timing: increase combo counter
    //                 comboCounter++;
    //                 hitCounter++; // Increment hit counter
    //                 lastPressTime = Time.time;
    //                 Debug.Log($"Perfect timing! Combo count: {comboCounter}");
                    
    //                 // Increase damage output
    //                 playerManager.TakeDamage(damage);

    //                 // Check if we should instantiate a combo note every 2 hits
    //                 if (hitCounter == 2)
    //                 {
    //                     playerManager.playerUIManagement.InstantiateComboUIElement(ref comboCounter, ref lastPressTime);
    //                     hitCounter = 0; // Reset hit counter

    //                     comboNoteCounter++; // Increment combo note counter

    //                     // Activate a power note every 2 combo notes
    //                     if (comboNoteCounter == 2)
    //                     {
    //                         playerManager.playerUIManagement.UpdateComboBar(ref playerManager.playerUIManagement.powerNoteBarIndex, ref playerManager.playerUIManagement.powerNoteIndex, playerManager.playerUIManagement.totalPowerNotesActiveAtOnce, null, ref comboCounter, ref lastPressTime);
    //                         comboNoteCounter = 0; // Reset combo note counter
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 playerManager.playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
    //             }
    //         }
    //     }

    //     if (lastPressTime >= 0 && Time.time - lastPressTime > requiredTimeFrame + tolerance + 2f)
    //     {
    //         playerManager.playerUIManagement.ResetComboSystem(ref comboCounter, ref lastPressTime);
    //     }

    // }
}
