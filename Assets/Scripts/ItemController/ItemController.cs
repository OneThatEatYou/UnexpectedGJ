using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour
{
    public AnimationClip idleClip;
    public AnimationClip walkClip;
    public AnimationClip slapClip;
    public AudioClip useSFX;
    public float useCooldown;

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void UseItem()
    {
        if (useSFX)
        {
            AudioManager.PlayAudioAtPosition(useSFX, transform.position, AudioManager.battleSfxMixerGroup);
        }
    }
}
