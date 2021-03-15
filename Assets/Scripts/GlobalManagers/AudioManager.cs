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
    public static AudioMixerGroup battleSfxMixerGroup;

    public void OnInit()
    {
        mainMixer = Resources.Load<AudioMixer>("Mixer/MainMixer");
        bgmMixerGroup = mainMixer.FindMatchingGroups("BGM")[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
        battleSfxMixerGroup = mainMixer.FindMatchingGroups("SFX/Battle")[0];
    }

    public static AudioSource PlayAudioAtPosition(AudioClip audioClip, Vector2 position, AudioMixerGroup mixerGroup, bool autoDestroy = true)
    {
        GameObject obj = new GameObject("OneShotAudio");
        obj.transform.position = position;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.spatialBlend = 0;
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();

        if (autoDestroy)
        {
            GameObject.Destroy(obj, audioClip.length);
        }

        return source;
    }

    //convert vol percent to value in mixer group
    public float VolumeToAtten(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
    }

    //convert value in mixer group to vol percent
    public float AttenToVol(float atten)
    {
        return Mathf.Pow(10, atten / 20);
    }
}
