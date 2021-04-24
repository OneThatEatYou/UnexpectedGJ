using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMLooper : MonoBehaviour
{
    public AudioSource source0;         //this source starts with intro
    public AudioSource source1;
    public AudioClip introAudioClip;
    public AudioClip loopableAudioClip;

    int flip = 1;

    private void Start()
    {
        StartCoroutine(PlayLoopingAudio(introAudioClip, loopableAudioClip, 0f));
    }

    IEnumerator PlayLoopingAudio(AudioClip introClip, AudioClip loopingClip, float initialDelay)
    {
        //schedule the intro clip to play
        double introLength = (double)introAudioClip.samples / introAudioClip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        source0.clip = introAudioClip;
        source0.PlayScheduled(startTime);

        //calculate the time for looping clip to play
        double scheduledTime = startTime + introLength;

        while (true)
        {
            //get the idle audio source and assign clip
            AudioSource source = GetVacantAudioSource();
            source.clip = loopableAudioClip;
           
            //schedule source to be played
            source.PlayScheduled(scheduledTime);

            //calculate the next time to play
            double loopableLength = (double)loopableAudioClip.samples / loopableAudioClip.frequency;
            scheduledTime += loopableLength;

            //wait until the occupied source finished playing
            yield return new WaitForSeconds((float)loopableLength + 1f);
            //Debug.Log("Finished waiting");
        }
    }

    AudioSource GetVacantAudioSource()
    {
        AudioSource source;

        if (flip == 0)
        {
            source = source0;
        }
        else
        {
            source = source1;
        }

        //change vacant audio source
        flip = 1 - flip;

        return source;
    }
}
