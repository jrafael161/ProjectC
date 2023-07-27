using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RedirectionVector
{
   Up,
   Forward,
   Right
}

public class SpecialBouncer : MonoBehaviour
{
    [SerializeField]
    float _bounceStrenght;
    [SerializeField]
    RedirectionVector _bounceDirection;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<RolyPolyManager>())
        {
            Vector3 velocityRedirectionVector = new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x, collision.gameObject.GetComponent<Rigidbody>().velocity.y, Mathf.Abs(collision.gameObject.GetComponent<Rigidbody>().velocity.z));
            Vector3 resultingVector = (velocityRedirectionVector + GetBounceDirection()) * _bounceStrenght;//Instead of having hardcoded the vector to take into consideration it could be passed from the editor or at least have options

            if (collision.gameObject.GetComponent<RolyPolyManager>().PinballManager.cameraMode == CameraMode.TopView)
            {
                resultingVector = Vector3.ProjectOnPlane(resultingVector,transform.forward);
            }
            else if (collision.gameObject.GetComponent<RolyPolyManager>().PinballManager.cameraMode == CameraMode.FrontView)
            {
                resultingVector = Vector3.ProjectOnPlane(resultingVector,transform.up);
            }

            Debug.DrawRay(collision.transform.position, resultingVector, Color.red, 3f);

            if (collision.gameObject.GetComponent<RolyPolyManager>().CheckVelocity(resultingVector))
                return;
            
            collision.gameObject.GetComponent<Rigidbody>().AddForce(resultingVector, ForceMode.Impulse);
        }
    }

    private Vector3 GetBounceDirection()
    {
        switch (_bounceDirection)
        {
            case RedirectionVector.Up:
                Debug.DrawRay(transform.position, transform.up, Color.green, 3f);
                return transform.up;
            case RedirectionVector.Forward:
                Debug.DrawRay(transform.position, transform.forward, Color.green, 3f);
                return transform.forward;
            case RedirectionVector.Right:
                Debug.DrawRay(transform.position, transform.right, Color.green, 3f);
                return transform.right;
            default:
                return transform.up;
        }
    }
}
