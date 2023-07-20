using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCompression : MonoBehaviour
{
    [SerializeField]
    Transform _FixedPoint;
    [SerializeField]
    Transform _MovingPoint;
    
    float _initialDistance;
    float _scaleFactor;

    private void Start()
    {
        _initialDistance = Vector3.Distance(_MovingPoint.position, _FixedPoint.position);
    }

    private void Update()
    {
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        _scaleFactor = Vector3.Distance(_MovingPoint.position, _FixedPoint.position) / _initialDistance;
        transform.localScale = new Vector3(1, _scaleFactor, 1);
    }
}
