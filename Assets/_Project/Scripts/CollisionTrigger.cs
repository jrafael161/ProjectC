using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent unityEvent;

    private void Start()
    {
        if (unityEvent == null)
        {
            unityEvent = new UnityEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<RolyPolyManager>())
        {
            unityEvent.Invoke();
        }
    }

    //Reparent the rolypoly when entering a vertical zone or when entering the pinball again so it doesnt skew its axis of rotation
}
