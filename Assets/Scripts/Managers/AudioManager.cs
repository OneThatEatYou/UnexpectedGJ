using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager
{
    public AudioMixer mainMixer;
    public static AudioMixerGroup bgmMixerGroup;
    public static AudioMixerGroup sfxMixerGroup;

    public void OnInit()
    {
        mainMixer = Resources.Load<AudioMixer>("Mixer/MainMixer");
        bgmMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
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

        GameObject.Destroy(obj, audioClip.length);

        return source;
    }
}
