using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
TO DO LIST:

-short/hop full hop sometime
-grounded friction during moves or something like that
-crouching (need sprite)

*/
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isHitstopOn, inHitStop;
    private Rigidbody2D[] rigidbodies2D;
    private Animator[] animators;
    [SerializeField] private int fps = 60;
    [SerializeField] int perfectTimingFrameWindow, goodTimingFrameWindow, badTimingFrameWindow;
    int currentFrame;
    [SerializeField] PianoMan pianoMan, dummy;
    [SerializeField] public AudioSource audioSource, snare, breakingTheHabit;
    Vector3 dummyStartPos;

    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
        StartCoroutine(MusicTiming());
    }
    void Start()
    {
        dummyStartPos = dummy.transform.position;
        isHitstopOn = true;
        QualitySettings.vSyncCount = 0;  // Disable VSync
        Application.targetFrameRate = fps; // Set target frame rate to 60 FPS

        // Get all Rigidbodies in the scene (or specific ones if needed)
        rigidbodies2D = FindObjectsOfType<Rigidbody2D>();
        animators = FindObjectsOfType<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Rigidbody2D dummyRB = dummy.GetComponent<Rigidbody2D>();
            dummy.transform.position = dummyStartPos;
            dummyRB.angularVelocity = 0;
            dummyRB.velocity = Vector3.zero;
        }
    }

    public IEnumerator HandleHitStop(float frames, float beats)
    {

        inHitStop = true;
        // Store the velocities to restore later
        Vector2[] originalVelocities = new Vector2[rigidbodies2D.Length];
        float[] originalAngularVelocities = new float[rigidbodies2D.Length];
        //Pause the animators
        foreach(Animator anim in animators)
        {
            anim.speed = 0;
        }

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
        foreach(Animator anim in animators)
        {
            anim.speed = 1;
        }
        inHitStop = false;
    }

    IEnumerator MusicTiming()
    {
        while(true)
        {
            for(int i2 = 0; i2 < 36; i2++) 
            {
                currentFrame = i2;
                if(currentFrame == 0) 
                {
                    Debug.Log("Metronome beat");
                    if(!breakingTheHabit.isPlaying) breakingTheHabit.Play();    
                    audioSource.Play();
                    //if(pianoMan.stateMachine != null) pianoMan.InitiateAttack();
                }
                //Debug.Log("Current frame: " + currentFrame);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public string GetAccuracyOnBeat()
    {
        int accuracy = 0;

        if(currentFrame <= 18)
        {
            accuracy = currentFrame;
        }
        else if (currentFrame <= 35)
        {
            accuracy = 36 - currentFrame;
        }
        else throw new System.Exception("no accuracy for this int");

        //Debug.Log("Off beat: " + currentFrame + " , "  + accuracy);

        if(accuracy  * 2 <= perfectTimingFrameWindow - 1)
        {
            Debug.Log("Accuracy: Perfect");
            return "Perfect";
        }
        else if(accuracy * 2 <= goodTimingFrameWindow - 1)
        {
            Debug.Log("Accuracy: OK");
            return "OK";
        }
        else if(accuracy == 18)
        {
            Debug.Log("Accuracy: Bad");
            return "Bad";
        }
        else if(accuracy * 2 <= badTimingFrameWindow - 1)
        {
            Debug.Log("Accuracy: Bad");
            return "Bad";
        }

        throw new System.Exception("no accuracy for this int");
    }
}
