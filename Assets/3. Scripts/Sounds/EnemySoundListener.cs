using UnityEngine;

public class EnemySoundListener : SoundListener
{
    private AudioSource audioSource;

    private void SoundPlaySFX(SoundEventSO so)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(so.clip);
    }
    private void SoundPlayBGM(SoundEventSO so)
    {
        audioSource = GetComponent<AudioSource>();
        var bgmClips = so.clip;
        if (audioSource.clip != null)
            audioSource.Stop();

        audioSource.clip = bgmClips;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlaySound(SoundEventSO so)
    {
        if (so.soundType == SoundType.SFX)
            SoundPlaySFX(so);
        else
            SoundPlayBGM(so);
    }

    public void WalkingSound()
    {
        //soSoundDatas[index].Raise();
    }
}