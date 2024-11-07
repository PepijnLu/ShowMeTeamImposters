using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerUIManagement
{
    // HP Bar
    [Header("HP Bar")]    
    public GameObject healthBarParent;
    public List<DictionaryEntry<GameObject, List<GameObject>>> HPBars = new();   
    private int healthBarLayerIndex = 0; 


    // HP Bar Combo Notes
    [Header("HP Bar Combo Notes")]
    public GameObject comboNotesParent;
    public List<DictionaryEntry2<GameObject, List<GameObject>, List<GameObject>>> comboNotesHealthBars = new();

    public const int maxLayers = 4;  // Set max number of layers or slots
    public int totalPowerNotesActiveAtOnce = 4;
    public int powerNoteIndex = 0;
    public int powerNoteBarIndex = 0;
    public int comboMeterIndex = 5;


    // Combat Combo Notes
    [Header("Combat Combo Notes")]
    [SerializeField] private GameObject combatComboNotePrefab; 
    [Tooltip("Point to instnatiate the UI Combo element")] [SerializeField] private Transform instantiatePoint; // Point to instnatiate the UI element
    [SerializeField] private float lerpDuration;
    [Tooltip("Temp storage")] public List<GameObject> comboNotes = new();
    // public List<GameObject> comboNotesPerSlot = new(); // List per slot
    [SerializeField] private float zRotationRange = 10f;


    // Combo Meter
    [Header("Combo Meter")]
    public GameObject comboMeterParent;
    public List<DictionaryEntry<string, GameObject>> comboMeters = new();


    public void CustomStart() 
    {
        Debug.Log($"Start -> Current layer index: {healthBarLayerIndex}");

        // Ensure each entry has a properly initialized list
        comboNotesHealthBars.Clear(); // Clear existing entries to avoid duplication
        for (int i = 0; i < maxLayers; i++)
        {
            comboNotesHealthBars.Add(new DictionaryEntry2<GameObject, List<GameObject>, List<GameObject>>
            {
                Key = null,
                Value = new List<GameObject>(), 
                Value_2 = new List<GameObject>() // Initialize with an empty list to prevent null references
            });
        }
    }

    public void InstantiatePlayerUI(GameObject UIPrefab, Transform UIPrefabParent) 
    {
        // Instantiate the prefab
        GameObject playerUI = Object.Instantiate(UIPrefab);
        playerUI.transform.SetParent(UIPrefabParent); // Set the transform under the UI parent

        // Adjust the rect transform
        RectTransform rectTransform = playerUI.GetComponent<RectTransform>(); // Fetch the RectTransform component
        Vector2 newPosition = new(50f, 100f); // UI Element Placement

        rectTransform.anchoredPosition = newPosition; // Assign the new position
        rectTransform.localScale = new Vector3(1f, 1f, 1f);

        // Initialize Combo-Meter
        int[] comboMeterLayers = new int[] { 0, 1, 2, 3, 4 }; 
        InitializeUIElement(ref comboMeterParent, playerUI, 0, comboMeterLayers, comboMeters, null); 

        // Initialize Health Bar
        int[] healthBarLayers = new int[] { 0, 1, 2, 3 }; 
        InitializeUIElement(ref healthBarParent, playerUI, 1, healthBarLayers, HPBars, null, true);

        // Initialize Combo Notes
        int[] comboNotesLayers = new int[] { 0, 1, 2, 3 }; 
        InitializeUIElement(ref comboNotesParent, playerUI, 2, comboNotesLayers, null, comboNotesHealthBars);
    }

    public void InitializeUIElement<T, U>
    (
        ref GameObject parent, 
        GameObject prefab, 
        int parentIndex, 
        int[] parentLayers, 
        List<DictionaryEntry<T, U>> entry,
        List<DictionaryEntry2<T, U, U>> entry_2, 
        bool createFullList = false
    ) 
    {
        parent = GetGameObject(prefab.transform, new int[] {0, parentIndex });
        foreach (var index in parentLayers)
        {
            GameObject layer = GetGameObject(parent.transform, new int[] { index });

            if(typeof(U) == typeof(GameObject)) 
            {
                if(typeof(T) == typeof(string)) 
                {
                    entry.Add(new DictionaryEntry<T, U> 
                    { 
                        Key = (T)(object)$"layer{index}", 
                        Value = (U)(object)layer
                    });
                }
            }
            else if(typeof(U) == typeof(List<GameObject>)) 
            {
                
                if(entry != null)  
                {
                    List<GameObject> tempStorage = new();
           
                    if(createFullList) 
                    {
                        int[] tempLayer = new int[] { 0, 1, 2, 3};
                        foreach (var index_2 in tempLayer)
                        {
                            GameObject childObject = GetGameObject(layer.transform, new int[] { index_2 });
                            tempStorage.Add(childObject);
                        }
                    }
                    
                    entry.Add(new DictionaryEntry<T, U> 
                    { 
                        Key = (T)(object)layer, 
                        Value = (U)(object)tempStorage
                    });
                }
            }

            if(entry_2 != null) 
            {
                if(typeof(U) == typeof(List<GameObject>)) 
                {
                    List<GameObject> tempStorage_2 = new();

                    if(!createFullList) 
                    {
                        int[] tempLayer_2 = new int[] { 0, 1, 2, 3 };
                        foreach (var index_3 in tempLayer_2)
                        {
                            GameObject childObject_2 = GetGameObject(layer.transform, new int[] { index_3 });
                            tempStorage_2.Add(childObject_2);
                        }

                        entry_2.Add(new DictionaryEntry2<T, U, U> 
                        { 
                            Key = (T)(object)layer, 
                            Value = (U)(object)tempStorage_2,
                            Value_2 = (U)(object)new List<GameObject>()
                        });
                        
                    }

                }
            }
        }
    }

    public GameObject GetGameObject(Transform parent, int[] path) 
    {
        foreach (var index in path)
        {
            parent = parent.GetChild(index);
        }

        return parent.gameObject;
    }


    public void UpdateHealthBar(float HP, ref float layerHP) 
    {        
        Debug.Log($"LayerHP: {layerHP}");
        if (HP > 0) 
        {
            float notesLeftInLayer = layerHP / 2f;
            UpdateNotesInLayer(healthBarLayerIndex, notesLeftInLayer); // Update notes based on remaining HP in the current layer

            // Check if we need to switch to a new layer
            if (layerHP <= 0 && healthBarLayerIndex < 3) 
            {
                SwitchToLayer(healthBarLayerIndex + 1);
                layerHP = 10; // Reset layer HP for the new layer
            }
        } 
        else if (HP == 0) 
        {
            // Special case: Deactivate all notes in the last layer when HP is zero
            if (healthBarLayerIndex == 3)
            {
                UpdateNotesInLayer(healthBarLayerIndex, 0);
            }
        }
    }

    public void UpdateComboBar(ref int powerNoteBarIndex, ref int powerNotesIndex, int powerNotesPerBar, GameObject UI_element, ref int comboCounter, ref float lastPressTime, bool isLayerReadyToSwitch = false) 
    {
        if (powerNoteBarIndex >= comboNotesHealthBars.Count)
            return;

        Debug.Log($"Current Power Note Bar Index: {powerNoteBarIndex}");

        var currentPowerNoteBar = comboNotesHealthBars[powerNoteBarIndex].Key;
        var powerNotes = comboNotesHealthBars[powerNoteBarIndex].Value;
        var comboNotes = comboNotesHealthBars[powerNoteBarIndex].Value_2;

        // Safety check: ensure powerNotes and comboNotes lists are properly initialized
        if (powerNotes == null || comboNotes == null)
        {
            Debug.LogError("Power notes or combo notes list is null.");
            return;
        }

        // Add the new UI element to combo notes if less than 2 are present
        if (comboNotes.Count < 2) 
        {
            comboNotes.Add(UI_element);
        }

        // When there are two combo notes, process the power note activation
        if (comboNotes.Count == 2)
        {
            if (powerNotesIndex < powerNotes.Count) // Ensure we're within bounds
            {
                GameObject currentPowerNote = powerNotes[powerNotesIndex];
                if (currentPowerNote != null)
                {
                    currentPowerNote.SetActive(true);
                    Debug.Log($"Activated Power Note at index {powerNotesIndex} in bar {powerNoteBarIndex}");
                }
                else
                {
                    Debug.LogError($"Power note at index {powerNotesIndex} is null.");
                }

                // Deactivate all other combo notes
                foreach (var element in comboNotes)
                {
                    if (element != null)
                    {
                        element.SetActive(false);
                    }
                }

                comboNotes.Clear();
                powerNotesIndex++;
            }

            // Check if we're at the limit of power notes
            if (powerNotesIndex == powerNotesPerBar) 
            {
                isLayerReadyToSwitch = true; // Mark the layer as ready to switch
                Debug.Log($"Fourth Power Note activated. Layer ready to switch on next hit.");
            }
        }

        // Only check for layer switch on the next hit
        if (isLayerReadyToSwitch && powerNotesIndex >= powerNotes.Count)
        {
            // Prepare to move to the next layer on the next hit
            if (currentPowerNoteBar != null)
            {
                currentPowerNoteBar.SetActive(false);
            }
            powerNoteBarIndex++;

            if (powerNoteBarIndex < comboNotesHealthBars.Count)
            {
                var nextPowerNoteBar = comboNotesHealthBars[powerNoteBarIndex].Key;
                if (nextPowerNoteBar != null)
                {
                    nextPowerNoteBar.SetActive(true);
                }

                powerNotesIndex = 0; // Reset index for new layer
                comboNotesHealthBars[powerNoteBarIndex].Value_2.Clear();
                isLayerReadyToSwitch = false; // Reset flag after switching

                Debug.Log($"Moved to next Power Note Bar. New Bar Index: {powerNoteBarIndex}");
            }
            else 
            {
                Debug.Log("All power note bars are fully activated.");
                UpdateComboMeter(ref comboMeterIndex);
                ResetComboSystem(ref comboCounter, ref lastPressTime);
            }
        }
    }




    private void UpdateComboMeter(ref int layerIndex, bool isComboMeterFull = false) 
    {
        if (layerIndex < 0 || layerIndex >= comboMeters.Count)
        {
            Debug.LogError($"Invalid layer index: {layerIndex}");
            return;
        }

        var element = comboMeters[layerIndex].Value;
        element.SetActive(false); // Deactivate the combo meter layer
        Debug.Log($"Combo Meter Layer Deactivated: {layerIndex}");

        layerIndex--; 
        Debug.Log($"Switched to layer index: {layerIndex}");

        if (layerIndex < 0) 
        {
            layerIndex = 4; // Keep layerIndex within bounds
            isComboMeterFull = true;
        }

        if(isComboMeterFull) 
        {
            foreach (var item in comboMeters)
            {
                item.Value.SetActive(true);
            }
        }
    }

    private void UpdateNotesInLayer(int layerIndex, float notesToActivate)
    {
        if (layerIndex >= HPBars.Count)
            return;

        var notes = HPBars[layerIndex].Value;

        // Activate all notes first, then deactivate based on the HP level
        for (int i = 0; i < notes.Count; i++)
        {
            int reverseIndex = notes.Count - 1 - i;
            notes[reverseIndex].SetActive(i < notesToActivate); // Activate notes based on remaining HP
        }
    }
    

    private void SwitchToLayer(int newLayerIndex)
    {
        if (healthBarLayerIndex != newLayerIndex)
        {
            // Deactivate current layer and its notes
            foreach (var note in HPBars[healthBarLayerIndex].Value)
            {
                note.SetActive(false);
            }

            HPBars[healthBarLayerIndex].Key.SetActive(false); // Deactivate the current health bar layer

            healthBarLayerIndex = newLayerIndex; // Update current layer index

            // Activate new layer and its notes
            HPBars[healthBarLayerIndex].Key.SetActive(true); // Activate the new health bar layer

            foreach (var note in HPBars[healthBarLayerIndex].Value)
            {
                note.SetActive(true);
            }
        }
    }

    public void InstantiateComboUIElement(ref int comboCounter, ref float lastPressTime) 
    {
        // Create a new combo UI element
        GameObject comboElement = Object.Instantiate(combatComboNotePrefab, instantiatePoint.position, Quaternion.identity);
        comboElement.transform.SetParent(instantiatePoint.transform);

        // Fetch and configure the RectTransform component
        RectTransform rectTransform = comboElement.GetComponent<RectTransform>();

        // Set anchors to fully stretch within parent
        rectTransform.anchorMin = Vector2.zero;    // Bottom-left corner of the parent
        rectTransform.anchorMax = Vector2.one;     // Top-right corner of the parent

        // Set offsets to zero to fully stretch and match the parent
        rectTransform.offsetMin = Vector2.zero;    // Left and Bottom
        rectTransform.offsetMax = Vector2.zero;    // Right and Top

        rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f); 

        // Randomize the z-axis rotation
        // Quaternion randomRotation = GenerateRandom2DRotation(zRotationRange);

        // Get the current target position for the UI element
        // Vector3 targetPosition = comboNotesHealthBar[currentNoteIndex].Value.transform.position;
        
        // Move the UI element towards the current note position
        // StartLerping(comboElement, instantiatePoint.position, targetPosition, Quaternion.identity, randomRotation, lerpDuration, monoBehaviour);
        
        // Store the UI element temporarily
        comboNotes.Add(comboElement); // Temp storage of total amount of combo notes
        
        UpdateComboBar(ref powerNoteBarIndex, ref powerNoteIndex, totalPowerNotesActiveAtOnce, comboElement, ref comboCounter, ref lastPressTime);
 
    }

    private Quaternion GenerateRandom2DRotation(float zRotationRange)
    {
        // Randomize rotation within the z-axis range
        float randomZ = Random.Range(-zRotationRange, zRotationRange);

        // Return a rotation around the z-axis only
        return Quaternion.Euler(0f, 0f, randomZ);
    }

    private void StartLerping(GameObject obj, Vector2 startPosition, Vector2 endPosition, Quaternion startRotation, Quaternion endRotation, float duration, MonoBehaviour monoBehaviour) 
    {
        monoBehaviour.StartCoroutine(TransformLerp(obj, startPosition, endPosition, duration));
        monoBehaviour.StartCoroutine(RotateLerp(obj, startRotation, endRotation, duration));
    }

    private IEnumerator TransformLerp(GameObject obj, Vector2 startPosition, Vector2 endPosition, float duration) 
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration) 
        {
            float t = elapsedTime / duration;
            
            if(obj == null) yield break;
            
            obj.transform.localPosition = Vector2.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = endPosition;
    }

    private IEnumerator RotateLerp(GameObject obj, Quaternion startRotation, Quaternion endRotation, float duration) 
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration) 
        {
            float t = elapsedTime / duration;
         
            if(obj == null) yield break;
         
            obj.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localRotation = endRotation;
    }

    public void ResetComboSystem(ref int comboCounter, ref float lastPressTime)
    {
        comboCounter = 0;
        lastPressTime = -1f;
        Debug.Log("Combo reset.");

        foreach (var element in comboNotesHealthBars)
        {
            ClearList(element.Value_2);
            foreach (var powerNote in element.Value)
            {
                powerNote.SetActive(false); // Deactivate each power note
            }
            element.Key.SetActive(false); // Deactivate each power note bar
        }
        
        comboNotes.Clear();
        powerNoteBarIndex = 0;
        powerNoteIndex = 0;
        comboNotesHealthBars[0].Key.SetActive(true);
    }

    public void ClearList<T>(List<T> list) 
    {
        bool destroyed = false;
        foreach (var element in list)
        {
            if(element is GameObject item) 
            {
                Object.Destroy(item);
                destroyed = true;
            }
        }

        if(list.Count > 0 && destroyed) 
        {
            list.Clear();
        }

        // Reset layer index for the combat combo notes
        powerNoteIndex = 0;
        powerNoteBarIndex = 0;

        // Deactivate the combo notes on the health bar
        for (int i = 0; i < comboNotesHealthBars.Count; i++)
        {
            var element = comboNotesHealthBars[i];
            element.Key.SetActive(false);
        }
    }

}

[System.Serializable]
public class DictionaryEntry<TKey, TValue> 
{
    public TKey Key;
    public TValue Value;
}

[System.Serializable]
public class DictionaryEntry2<TKey, TValue, TValue_2> 
{
    public TKey Key;
    public TValue Value;
    public TValue_2 Value_2;
}

    // public void UpdateComboBar(ref int powerNoteBarIndex, ref int powerNotesIndex, int powerNotesPerBar, GameObject UI_element, ref int comboCounter, ref float lastPressTime, bool isLayerReadyToSwitch = false) 
    // {
    //     if (powerNoteBarIndex >= comboNotesHealthBars.Count)
    //         return;

    //     Debug.Log($"Current Power Note Bar Index: {powerNoteBarIndex}");

    //     var currentPowerNoteBar = comboNotesHealthBars[powerNoteBarIndex].Key;
    //     var powerNotes = comboNotesHealthBars[powerNoteBarIndex].Value;
    //     var comboNotes = comboNotesHealthBars[powerNoteBarIndex].Value_2;

    //     // Add the new UI element to combo notes if less than 2 are present
    //     if (comboNotes.Count < 2) 
    //     {
    //         comboNotes.Add(UI_element);
    //     }

    //     // When there are two combo notes, process the power note activation
    //     if (comboNotes.Count == 2)
    //     {
    //         if (powerNotesIndex < powerNotes.Count) // Ensure we're within bounds
    //         {
    //             GameObject currentPowerNote = powerNotes[powerNotesIndex];
    //             currentPowerNote.SetActive(true);
    //             Debug.Log($"Activated Power Note at index {powerNotesIndex} in bar {powerNoteBarIndex}");

    //             Debug.Log($"Combo Notes Count: {comboNotes.Count}");
    //             // Deactivate all other combo notes
    //             foreach (var element in comboNotes)
    //             {
    //                 element.SetActive(false);
    //             }

    //             comboNotes.Clear();
    //             powerNotesIndex++;
    //         }

    //         // Check if we're at the limit of power notes
    //         if (powerNotesIndex == powerNotesPerBar) 
    //         {
    //             isLayerReadyToSwitch = true; // Mark the layer as ready to switch
    //             Debug.Log($"Fourth Power Note activated. Layer ready to switch on next hit.");
    //         }
    //     }

    //     // Only check for layer switch on the next hit
    //     if (isLayerReadyToSwitch && powerNotesIndex >= powerNotes.Count)
    //     {
    //         // Prepare to move to the next layer on the next hit
    //         currentPowerNoteBar.SetActive(false);
    //         powerNoteBarIndex++;

    //         if (powerNoteBarIndex < comboNotesHealthBars.Count)
    //         {
    //             var nextPowerNoteBar = comboNotesHealthBars[powerNoteBarIndex].Key;
    //             nextPowerNoteBar.SetActive(true);
    //             powerNotesIndex = 0; // Reset index for new layer
    //             comboNotesHealthBars[powerNoteBarIndex].Value_2.Clear();
    //             isLayerReadyToSwitch = false; // Reset flag after switching

    //             Debug.Log($"Moved to next Power Note Bar. New Bar Index: {powerNoteBarIndex}");
    //         }
    //         else 
    //         {
    //             Debug.Log("All power note bars are fully activated.");
    //             UpdateComboMeter(ref comboMeterIndex);
    //             ResetComboSystem(ref comboCounter, ref lastPressTime);
    //         }
    //     }
    // }