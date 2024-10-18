using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // Disable VSync
        Application.targetFrameRate = 60; // Set target frame rate to 60 FPS
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
