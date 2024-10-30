using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerUI
{
    // HP Bar
    [Header("Health Bar")]    
    public GameObject HealthBarParent;
    public List<DictionaryEntry<string, GameObject>> healthBars = new();   
    public List<HealthBarNoteLayer> healthBarNotes = new(); 
    private int currentLayerIndex = 0; 

    // Combo Notes
    [Header("ComboNotes")]
    public GameObject ComboNotesParent;
    public List<DictionaryEntry<string, GameObject>> comboNotes = new(); 

    // Combo Meter
    [Header("ComboMeter")]
    public GameObject ComboMeterParent;
    public List<DictionaryEntry<string, GameObject>> comboMeters = new();

    public void CustomStart() 
    {
        Debug.Log($"Start -> Current layer index: {currentLayerIndex}");
        Debug.Log($"Start -> Amount of notes in the layer {healthBarNotes[currentLayerIndex].notes.Count}");
    }


    public void InstantiatePlayerUI(GameObject UIPrefab, Transform UIPrefabParent) 
    {
        // Instantiate the prefab
        GameObject playerUI = Object.Instantiate(UIPrefab);
        playerUI.transform.SetParent(UIPrefabParent); // Set the transform under the UI parent

        // Adjust the rect transform
        RectTransform rectTransform = playerUI.GetComponent<RectTransform>(); // Fetch the RectTransform component
        Vector2 newPosition = new(25f, 60f); // UI Element Placement

        rectTransform.anchoredPosition = newPosition; // Assign the new position

        // Initialize Combo-Meter
        int[] comboMeterLayers = new int [] { 0, 1, 2, 3, 4 }; 
        InitializeUIElements(ref ComboMeterParent, playerUI, 0, comboMeterLayers, comboMeters); 

        // Initialize Health Bar
        int[] healthBarLayers = new int [] { 0, 1, 2, 3 }; 
        InitializeUIElements(ref HealthBarParent, playerUI, 1, healthBarLayers, healthBars);

        // Initialize Combo Notes
        int[] comboNotesLayers = new int [] { 0, 1, 2, 3 }; 
        InitializeUIElements(ref ComboNotesParent, playerUI, 2, comboNotesLayers, comboNotes);

        InitializeHealthBarNotes();
    }

    public void InitializeUIElements
    (
        ref GameObject parent, 
        GameObject prefab, 
        int parentIndex, 
        int[] parentLayers,
        List<DictionaryEntry<string, GameObject>> entry
    ) 
    {
        parent = GetGameObject(prefab.transform, new int [] { 0, parentIndex } );
        foreach (var index in parentLayers)
        {
            GameObject layer = GetGameObject(parent.transform, new int [] { index });
            entry.Add(new DictionaryEntry<string, GameObject> { Key = $"layer{index}", Value = layer});
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

    public void InitializeHealthBarNotes()
    {
        foreach (var layer in healthBars)
        {
            HealthBarNoteLayer noteLayer = new();

            foreach (Transform note in layer.Value.transform)
            {
                noteLayer.notes.Add(note.gameObject);
            }

            healthBarNotes.Add(noteLayer);
        }
    }

    public void UpdateHealthBar(int HP, ref float layerHP) 
    {
        // Debug.Log($"UpdateHealthBar -> HP: {HP}");
        
        if (HP > 0) 
        {
            // int notesLeftInLayer = (HP % 100) / 25;
            float notesLeftInLayer = layerHP / 25f;
            
            // Debug.Log($"UpdateHealthBar -> notesLeftInLayer calculation: {notesLeftInLayer}");
            // Debug.Log($"UpdateHealthBar -> Current layer index: {currentLayerIndex}");

            // Update the notes based on remaining HP in the current layer
            UpdateNotesInLayer(currentLayerIndex, notesLeftInLayer);

            // Check if we need to switch to a new layer
            if (layerHP <= 0 && currentLayerIndex < 3) 
            {
                SwitchToLayer(currentLayerIndex + 1);
                layerHP = 100; // Reset layer HP for the new layer
            }

            // // Check if we need to switch to a new layer
            // if (notesLeftInLayer == 0 && currentLayerIndex < 3) 
            // {
            //     SwitchToLayer(currentLayerIndex + 1);
            // }
        } 
        else if (HP == 0) 
        {
            // Debug.Log("UpdateHealthBar -> Handling zero HP, deactivating final layer notes.");
            
            // Special case: Deactivate all notes in the last layer when HP is zero
            if (currentLayerIndex == 3)
            {
                UpdateNotesInLayer(currentLayerIndex, 0);
            }
        }
    }

    private void UpdateNotesInLayer(int layerIndex, float notesToActivate)
    {
        var notes = healthBarNotes[layerIndex].notes;
        // Debug.Log($"UpdateNotesInLayer -> ListNumber: {layerIndex}");

        // Activate all notes first, then deactivate based on the HP level
        for (int i = 0; i < notes.Count; i++)
        {
            notes[i].SetActive(i < notesToActivate); // Activate notes based on the notes left
        }
    }

    private void SwitchToLayer(int newLayerIndex)
    {
        if (currentLayerIndex != newLayerIndex)
        {
            // Debug.Log($"SwitchToLayer -> CurrentLayerIndex: {currentLayerIndex}");

            // Deactivate current layer and its notes
            foreach (var note in healthBarNotes[currentLayerIndex].notes)
            {
                note.SetActive(false);
                // Debug.Log($"SwitchToLayer -> Name of note: {note.name}");
            }

            healthBars[currentLayerIndex].Value.SetActive(false);

            // Update current layer index
            currentLayerIndex = newLayerIndex;
            // Debug.Log($"SwitchToLayer -> CurrentLayerIndex: {currentLayerIndex}");

            // Activate new layer and its notes
            healthBars[currentLayerIndex].Value.SetActive(true);
            foreach (var note in healthBarNotes[currentLayerIndex].notes)
            {
                note.SetActive(true);
            }
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
public class HealthBarNoteLayer
{
    public List<GameObject> notes = new();
}