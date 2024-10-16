using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource baseAudioSource;
    [SerializeField] List<AudioSource> audioSourceList;
    AudioSource currentAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        currentAudioSource = audioSourceList[0];
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void SwitchSongLeft()
    {
        StartCoroutine(SwitchSong(true));
    }
    public void SwitchSongRight()
    {
        StartCoroutine(SwitchSong(false));
    }

    IEnumerator SwitchSong(bool left)
    {
        float currentTime = baseAudioSource.time;
        int indexOfSourceToPlay;

        if (left) indexOfSourceToPlay = audioSourceList.IndexOf(currentAudioSource) - 1;
        else indexOfSourceToPlay = audioSourceList.IndexOf(currentAudioSource) + 1;

        if (indexOfSourceToPlay < 0) indexOfSourceToPlay = audioSourceList.Count - 1;
        if (indexOfSourceToPlay >= audioSourceList.Count) indexOfSourceToPlay = 0;

        AudioSource sourceToPlay = audioSourceList[indexOfSourceToPlay];

        if(sourceToPlay == audioSourceList[0])
        {
            currentTime = currentTime * 1f;
        }
        if(sourceToPlay == audioSourceList[1])
        {
            currentTime = currentTime * 1.25f;
        }
        if(sourceToPlay == audioSourceList[2])
        {
            currentTime = currentTime * 0.75f;
        }

        currentAudioSource.Stop();
        sourceToPlay.time = currentTime;    
        sourceToPlay.Play();
        currentAudioSource = sourceToPlay;

        yield return null;
    }
}
