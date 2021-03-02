using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Slider masterVolSlider, bgmVolSlider, sfxVolSlider;
    public AudioClip[] sfxClips;
    AudioSource audioSource;

    public GameObject[] catalogPages;
    int currentPageInt = 0;

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
        vol = GameManager.Instance.audioManager.AttenToVol(vol);
        return vol;
    }

    public virtual void ChangeScene(int sceneIndex)
    {
        GameManager.Instance.ChangeScene(sceneIndex);
    }

    public void ChangeMasterVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_Master", GameManager.Instance.audioManager.VolumeToAtten(value));
    }
    
    public void ChangeBGMVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_BGM", GameManager.Instance.audioManager.VolumeToAtten(value));
    }

    public void ChangeSFXVolume(float value)
    {
        GameManager.Instance.audioManager.mainMixer.SetFloat("Vol_SFX", GameManager.Instance.audioManager.VolumeToAtten(value));
    }

    public void PlayRandomSFX()
    {
        AudioClip clip = sfxClips[Random.Range(0, sfxClips.Length)];
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void NextCatalogPage()
    {
        //check if next page exist
        if (currentPageInt + 1 < catalogPages.Length)
        {
            catalogPages[currentPageInt].SetActive(false);
            catalogPages[++currentPageInt].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"No page after {catalogPages[currentPageInt]} exists.");
        }
    }

    public void PrevCatalogPage()
    {
        //check if next page exist
        if (currentPageInt - 1 >= 0)
        {
            catalogPages[currentPageInt].SetActive(false);
            catalogPages[--currentPageInt].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"No page before {catalogPages[currentPageInt]} exists.");
        }
    }
}
