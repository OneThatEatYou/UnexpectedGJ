using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Slider masterVolSlider, bgmVolSlider, sfxVolSlider;
    public AudioClip[] sfxClips;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        masterVolSlider.value = GetCurrentVolume("Vol_Master");
        bgmVolSlider.value = GetCurrentVolume("Vol_BGM");
        sfxVolSlider.value = GetCurrentVolume("Vol_SFX");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit app");
    }

    float GetCurrentVolume(string paramName)
    {
        float vol;
        GameManager.Instance.audioManager.mainMixer.GetFloat(paramName, out vol);
        vol = AttenToVol(vol);
        return vol;
    }

    public virtual void ChangeScene(int sceneIndex)
    {
        GameManager.Instance.ChangeScene(sceneIndex);
    }

    public void ChangeMasterVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_Master", VolumeToAtten(value));
    }
    
    public void ChangeBGMVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_BGM", VolumeToAtten(value));
    }

    public void ChangeSFXVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_SFX", VolumeToAtten(value));
    }

    public void PlayRandomSFX()
    {
        AudioClip clip = sfxClips[Random.Range(0, sfxClips.Length)];
        audioSource.clip = clip;
        audioSource.Play();
    }

    //convert vol percent to value in mixer group
    float VolumeToAtten(float volume)
    {
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;
    }
    
    //convert value in mixer group to vol percent
    float AttenToVol(float atten)
    {
        return Mathf.Pow(10, atten / 20);
    }
}
