using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerBehaviour : MonoBehaviour
{
    [SerializeField]
    float _bounceStrenght;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RolyPolyManager>())
        {
            Vector3 resultingVector = collision.gameObject.GetComponent<Rigidbody>().velocity * _bounceStrenght;
            if (collision.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(resultingVector))
            {
                return;
            }
            collision.gameObject.GetComponent<Rigidbody>().AddForce(resultingVector,ForceMode.Impulse);
        }
    }
}
