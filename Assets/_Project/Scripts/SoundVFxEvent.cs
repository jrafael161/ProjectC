using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVFxEvent : MonoBehaviour , IRolyPolySoundEvents
{
    [SerializeField]
    AudioClip _bounceSound;
    [SerializeField]
    AudioSources _audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RolyPolyManager>())
        {
            PlaySound(_bounceSound, _audioSource);
        }
    }

    public void PlaySound(AudioClip audioClip, AudioSources audioSource)
    {
        MasterAudioManager._instance.PlaySound(audioSource, audioClip);
    }
}
