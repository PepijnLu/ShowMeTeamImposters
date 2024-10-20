using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isHitstopOn;
    private Rigidbody2D[] rigidbodies2D;
    [SerializeField] private int fps = 60;
    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
        StartCoroutine(MusicTiming());
    }
    void Start()
    {
        isHitstopOn = true;
        QualitySettings.vSyncCount = 0;  // Disable VSync
        Application.targetFrameRate = fps; // Set target frame rate to 60 FPS

        // Get all Rigidbodies in the scene (or specific ones if needed)
        rigidbodies2D = FindObjectsOfType<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator HandleHitStop(float frames)
    {
        // Store the velocities to restore later
        Vector2[] originalVelocities = new Vector2[rigidbodies2D.Length];
        float[] originalAngularVelocities = new float[rigidbodies2D.Length];

        // Stop the physics simulation and freeze objects in place
        for (int i = 0; i < rigidbodies2D.Length; i++)
        {
            originalVelocities[i] = rigidbodies2D[i].velocity;
            originalAngularVelocities[i] = rigidbodies2D[i].angularVelocity;

            // Set velocities to zero to make objects stay still
            rigidbodies2D[i].velocity = Vector2.zero;
            rigidbodies2D[i].angularVelocity = 0f;
        }

        // Pause the physics simulation (2D doesn't have SimulationMode, but we can manually control it)
        Physics2D.simulationMode = SimulationMode2D.Script;

        // Wait for the specified duration
        for(int i = 0; i < frames; i++) yield return new WaitForFixedUpdate();

        // Resume the physics simulation and restore the original velocities
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        for (int i = 0; i < rigidbodies2D.Length; i++)
        {
            rigidbodies2D[i].velocity = originalVelocities[i];
            rigidbodies2D[i].angularVelocity = originalAngularVelocities[i];
        }
    }

    IEnumerator MusicTiming()
    {
        int testLimit = 100;
        for(int i = 0; i < testLimit; i++)
        {
            Debug.Log("Metronome beat " + i);
            for(int i2 = 0; i2 < 36; i2++) yield return new WaitForFixedUpdate();
        }
    }
}
