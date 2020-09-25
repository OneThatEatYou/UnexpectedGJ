using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public static AudioMixerGroup bgmMixerGroup;
    public static AudioMixerGroup sfxMixerGroup;

    public AudioSource introSource;
    public AudioSource loopableSource;
    public AudioClip loopableAudioClip;

    double startTime;
    double startLoop;
    int flip = 0;

    private void Awake()
    {
        bgmMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
    }

    private void Start()
    {
        AudioClip introClip = introSource.clip;
        double introLength = (double)introClip.samples / introClip.frequency;
        startTime = AudioSettings.dspTime + 0.2;
        startLoop = startTime + introLength;
        //Debug.Log($"Start: {startTime}, LoopStart: {startLoop}");
        introSource.PlayScheduled(startTime);
        loopableSource.PlayScheduled(startLoop);
        StartCoroutine(KeepPlayingScheduled(loopableAudioClip, (float)startLoop + 1f - (float)AudioSettings.dspTime));
    }

    IEnumerator KeepPlayingScheduled(AudioClip clip, float initialDelay)
    {
        double scheduledTime = startLoop;

        Debug.Log("Initial delay: " + initialDelay);
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            //get the idle audoi source
            AudioSource source = GetVacantAudioSource();
            //assign audio clip to source
            source.clip = clip;
            //calculate clip length
            double clipLength = (double)clip.samples / clip.frequency;
            scheduledTime += clipLength;
            //schedule source to be played
            source.PlayScheduled(scheduledTime);
            //wait until the occupied source finished playing
            Debug.Log("Waiting for " + ((float)clipLength + 1f) + " seconds");
            yield return new WaitForSeconds((float)clipLength + 1f);
            Debug.Log("Finished waiting");
        }
    }

    AudioSource GetVacantAudioSource()
    {
        AudioSource source;

        if (flip == 0)
        {
            source = introSource;
        }
        else
        {
            source = loopableSource;
        }

        //change vacant audio source
        flip = 1 - flip;

        return source;
    }

    public static AudioSource PlayAudioAtPosition(AudioClip audioClip, Vector2 position, AudioMixerGroup mixerGroup)
    {
        GameObject obj = new GameObject("OneShotAudio");
        obj.transform.position = position;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.spatialBlend = 0;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        Destroy(obj, audioClip.length);

        return source;
    }
}
