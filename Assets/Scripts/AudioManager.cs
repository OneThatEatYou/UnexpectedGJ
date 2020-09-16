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

    private void Awake()
    {
        bgmMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
    }

    private void Start()
    {
        AudioClip introClip = introSource.clip;
        double introLength = (double)introClip.samples / introClip.frequency;
        double startTime = AudioSettings.dspTime + 0.2;
        double startLoop = startTime + introLength;
        //Debug.Log($"Start: {startTime}, LoopStart: {startLoop}");
        introSource.PlayScheduled(startTime);
        loopableSource.PlayScheduled(startLoop);
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
