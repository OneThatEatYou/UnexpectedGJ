using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GenerateImpulse(Vector2.down, 5, 5);
        //}
        //else if (Input.GetKeyDown(KeyCode.L))
        //{
        //    GenerateImpulse(Vector2.right, 5, 5);
        //}
        //else if (Input.GetKeyDown(KeyCode.J))
        //{
        //    GenerateImpulse(Vector2.left, 5 ,5);
        //}
        //else if (Input.GetKeyDown(KeyCode.I))
        //{
        //    GenerateImpulse(Vector2.up, 5 ,5);
        //}
    }

    public static void GenerateImpulse(Vector2 direction, float amplitudeGain, float frequencyGain, float attackTime = 0.1f, float sustainTime = 0.4f, float decayTime = 0.9f)
    {
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitudeGain;
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequencyGain;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = attackTime;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = sustainTime;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decayTime;
        impulseSource.GenerateImpulse(direction);
    }

    public static void GenerateImpulse(float amplitudeGain, float frequencyGain, float attackTime = 0.1f, float sustainTime = 0.4f, float decayTime = 0.9f)
    {
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitudeGain;
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequencyGain;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = attackTime;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = sustainTime;
        impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decayTime;
        impulseSource.GenerateImpulse(1f);
    }
}
