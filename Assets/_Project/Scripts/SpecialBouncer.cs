using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBouncer : MonoBehaviour
{
    [SerializeField]
    float _bounceStrenght;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RolyPolyManager>())
        {
            Vector3 velocityRedirectionVector = new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x, collision.gameObject.GetComponent<Rigidbody>().velocity.y, Mathf.Abs(collision.gameObject.GetComponent<Rigidbody>().velocity.z));
            Vector3 resultingVector = (velocityRedirectionVector - Vector3.right) * _bounceStrenght;//Instead of having hardcoded the vector to take into consideration ot could be passed from the editor or at least have options
            if (collision.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(resultingVector))
            {
                return;
            }
            collision.gameObject.GetComponent<Rigidbody>().AddForce(resultingVector, ForceMode.Impulse);
        }
    }
}
