using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerBehaviour : MonoBehaviour, IRolyPolySoundEvents
{
    [SerializeField]
    float _bounceStrenght;
    [SerializeField]
    AudioClip _bounceSound;
    [SerializeField]
    AudioSources _audioSource;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RolyPolyManager>())
        {
            PlaySound(_bounceSound, _audioSource);
            Vector3 resultingVector = collision.gameObject.GetComponent<Rigidbody>().velocity * _bounceStrenght;

            if (collision.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(resultingVector))
                return;
            
            collision.gameObject.GetComponent<Rigidbody>().AddForce(resultingVector,ForceMode.Impulse);
        }
    }

    public void PlaySound(AudioClip audioClip, AudioSources audioSource)
    {
        MasterAudioManager._instance.PlaySound(audioSource, audioClip);
    }
}
