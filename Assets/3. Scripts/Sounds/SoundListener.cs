using System;
using UnityEngine;
using UnityEngine.Events;

public class SoundListener : MonoBehaviour
{
    //private AudioSource audioSource;
    public SoundEventSO[] soSoundDatas;

    //public UnityAction action;
    [Serializable] public class SoundEvent : UnityEvent<SoundEventSO> { }
    public SoundEvent response;

    private void OnEnable()
    {
        foreach (var e in soSoundDatas)
            e.RegisterListener(this);
    }
    private void OnDisable()
    {
        foreach (var e in soSoundDatas)
            e.UnRegisterListener(this);
    }

    public void OnEventRaised(SoundEventSO soundEventSo)
    {
        response.Invoke(soundEventSo);
    }
}